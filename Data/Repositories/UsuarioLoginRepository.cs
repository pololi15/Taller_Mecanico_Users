using Npgsql;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_Mecanico_Users.Domain.Entities;
using Taller_Mecanico_Users.Domain.Common;
using Taller_Mecanico_Users.Domain.Ports;
using Taller_Mecanico_Users.Framework.Persistence;
using Taller_Mecanico_Users.Framework.Services;

namespace Taller_Mecanico_Users.Data.Repositories
{
    public class UsuarioLoginRepository : IUsuarioLoginRepository
    {

        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly IAuthenticationHelper _authHelper;

        public UsuarioLoginRepository(ISqlConnectionFactory connectionFactory, IAuthenticationHelper authHelper)
        {
            _connectionFactory = connectionFactory;
            _authHelper = authHelper;
        }

        public async Task<UsuarioLogin?> GetByEmailAsync(string email)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"
SELECT usuariologinid, empleadoid, clienteid, email, passwordhash, ultimoacceso, activo, requierecambiopassword, escliente
FROM usuariologin
WHERE email = @email AND activo = TRUE;";

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            var param = command.CreateParameter();
            param.ParameterName = "email";
            param.Value = email;
            command.Parameters.Add(param);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return ReconstituirFromReader(reader);
        }

        public async Task<IEnumerable<UsuarioLogin>> GetAllAsync()
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"
SELECT usuariologinid, empleadoid, clienteid, email, passwordhash, ultimoacceso, activo, requierecambiopassword, escliente
FROM usuariologin
ORDER BY email;";

            var logins = new List<UsuarioLogin>();
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                logins.Add(ReconstituirFromReader(reader));
            }

            return logins;
        }

        public async Task<Result<UsuarioLogin?>> GetByIdAsync(int id)
        {
            try
            {
                await using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var sql = @"
SELECT usuariologinid, empleadoid, clienteid, email, passwordhash, ultimoacceso, activo, requierecambiopassword, escliente
FROM usuariologin
WHERE usuariologinid = @id;";

                await using var command = connection.CreateCommand();
                command.CommandText = sql;
                var p = command.CreateParameter();
                p.ParameterName = "id";
                p.Value = id;
                command.Parameters.Add(p);

                await using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return Result<UsuarioLogin?>.Success(null);

                return Result<UsuarioLogin?>.Success(ReconstituirFromReader(reader));
            }
            catch (Exception ex)
            {
                return Result<UsuarioLogin?>.Failure(ErrorCodes.DbError, $"Error al obtener usuario login con ID {id}: {ex.Message}");
            }
        }

        public async Task<UsuarioLogin?> GetByEmpleadoIdAsync(int empleadoId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"
SELECT usuariologinid, empleadoid, clienteid, email, passwordhash, ultimoacceso, activo, requierecambiopassword, escliente
FROM usuariologin
WHERE empleadoid = @empleadoId AND activo = TRUE;";

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            var p2 = command.CreateParameter();
            p2.ParameterName = "empleadoId";
            p2.Value = empleadoId;
            command.Parameters.Add(p2);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return ReconstituirFromReader(reader);
        }

        public async Task<UsuarioLogin?> GetByClienteIdAsync(int clienteId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = @"
SELECT usuariologinid, empleadoid, clienteid, email, passwordhash, ultimoacceso, activo, requierecambiopassword, escliente
FROM usuariologin
WHERE clienteid = @clienteId AND activo = TRUE;";

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            var p3 = command.CreateParameter();
            p3.ParameterName = "clienteId";
            p3.Value = clienteId;
            command.Parameters.Add(p3);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return ReconstituirFromReader(reader);
        }

        public async Task<Result> AddAsync(UsuarioLogin entity)
        {
            try
            {
                await using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var actorAuditoria = _authHelper.GetCurrentAuditActor();

                var sql = entity.EsCliente
                    ? @"
INSERT INTO usuariologin (clienteid, email, passwordhash, activo, requierecambiopassword, escliente, creadopor)
VALUES (@clienteid, @email, @passwordhash, @activo, @requierecambiopassword, @escliente, @creadopor)
RETURNING usuariologinid;"
                    : @"
INSERT INTO usuariologin (empleadoid, email, passwordhash, activo, requierecambiopassword, escliente, creadopor)
VALUES (@empleadoid, @email, @passwordhash, @activo, @requierecambiopassword, @escliente, @creadopor)
RETURNING usuariologinid;";

                await using var command = connection.CreateCommand();
                command.CommandText = sql;

                if (entity.EsCliente)
                {
                    var cp = command.CreateParameter(); cp.ParameterName = "clienteid"; cp.Value = entity.ClienteId!.Value; command.Parameters.Add(cp);
                }
                else
                {
                    var ep = command.CreateParameter(); ep.ParameterName = "empleadoid"; ep.Value = entity.EmpleadoId!.Value; command.Parameters.Add(ep);
                }

                var pEmail = command.CreateParameter(); pEmail.ParameterName = "email"; pEmail.Value = entity.Email; command.Parameters.Add(pEmail);
                var pPass = command.CreateParameter(); pPass.ParameterName = "passwordhash"; pPass.Value = entity.PasswordHash; command.Parameters.Add(pPass);
                var pActivo = command.CreateParameter(); pActivo.ParameterName = "activo"; pActivo.Value = entity.Activo; command.Parameters.Add(pActivo);
                var pReq = command.CreateParameter(); pReq.ParameterName = "requierecambiopassword"; pReq.Value = entity.RequiereCambioPassword; command.Parameters.Add(pReq);
                var pEsCli = command.CreateParameter(); pEsCli.ParameterName = "escliente"; pEsCli.Value = entity.EsCliente; command.Parameters.Add(pEsCli);
                var pCreador = command.CreateParameter(); pCreador.ParameterName = "creadopor"; pCreador.Value = actorAuditoria; command.Parameters.Add(pCreador);

                var resultObj = await command.ExecuteScalarAsync();
                entity.UsuarioLoginId = Convert.ToInt32(resultObj);
                return Result.Success();
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Result.Failure(ErrorCodes.UsuarioEmailDuplicado, "El correo electrónico ya está registrado.");
            }
            catch (Exception ex)
            {
                return Result.Failure(ErrorCodes.DbError, $"Error al registrar usuario login: {ex.Message}");
            }
        }

        public async Task<Result> UpdateAsync(UsuarioLogin entity)
        {
            try
            {
                await using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                var sql = @"
UPDATE usuariologin
SET email = @email,
    passwordhash = @passwordhash,
    activo = @activo,
    ultimoacceso = @ultimoacceso,
    requierecambiopassword = @requierecambiopassword,
    actualizadopor = @actualizadopor
WHERE usuariologinid = @usuariologinid;";

                var actorAuditoria = _authHelper.GetCurrentAuditActor();

                await using var command = connection.CreateCommand();
                command.CommandText = sql;
                var pId = command.CreateParameter(); pId.ParameterName = "usuariologinid"; pId.Value = entity.UsuarioLoginId; command.Parameters.Add(pId);
                var pEmail2 = command.CreateParameter(); pEmail2.ParameterName = "email"; pEmail2.Value = entity.Email; command.Parameters.Add(pEmail2);
                var pPass2 = command.CreateParameter(); pPass2.ParameterName = "passwordhash"; pPass2.Value = entity.PasswordHash; command.Parameters.Add(pPass2);
                var pActivo2 = command.CreateParameter(); pActivo2.ParameterName = "activo"; pActivo2.Value = entity.Activo; command.Parameters.Add(pActivo2);
                var pUlt = command.CreateParameter(); pUlt.ParameterName = "ultimoacceso"; pUlt.Value = (object?)entity.UltimoAcceso ?? DBNull.Value; command.Parameters.Add(pUlt);
                var pReq2 = command.CreateParameter(); pReq2.ParameterName = "requierecambiopassword"; pReq2.Value = entity.RequiereCambioPassword; command.Parameters.Add(pReq2);
                var pAct = command.CreateParameter(); pAct.ParameterName = "actualizadopor"; pAct.Value = actorAuditoria; command.Parameters.Add(pAct);

                await command.ExecuteNonQueryAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ErrorCodes.DbError, $"Error al actualizar usuario login: {ex.Message}");
            }
        }

        private static UsuarioLogin ReconstituirFromReader(DbDataReader reader)
        {
            var ordinal = reader.GetOrdinal;
            var empleadoIdOrdinal = ordinal("empleadoid");
            var clienteIdOrdinal = ordinal("clienteid");

            return UsuarioLogin.Reconstituir(
                Convert.ToInt32(reader.GetValue(ordinal("usuariologinid"))),
                reader.IsDBNull(empleadoIdOrdinal) ? null : (int?)Convert.ToInt32(reader.GetValue(empleadoIdOrdinal)),
                reader.IsDBNull(clienteIdOrdinal) ? null : (int?)Convert.ToInt32(reader.GetValue(clienteIdOrdinal)),
                reader.GetString(ordinal("email")),
                reader.GetString(ordinal("passwordhash")),
                reader.IsDBNull(ordinal("ultimoacceso")) ? null : (DateTime?)Convert.ToDateTime(reader.GetValue(ordinal("ultimoacceso"))),
                Convert.ToBoolean(reader.GetValue(ordinal("activo"))),
                Convert.ToBoolean(reader.GetValue(ordinal("requierecambiopassword"))),
                Convert.ToBoolean(reader.GetValue(ordinal("escliente"))));
        }
    }
}
