using Taller_Mecanico_Users.Domain.Ports;
using Taller_Mecanico_Users.Domain.Common;

namespace Taller_Mecanico_Users.UseCases.Users
{
    public class ChangePasswordUseCase
    {
        private readonly IUsuarioLoginRepository _loginRepository;

        public ChangePasswordUseCase(IUsuarioLoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public async Task<Result> ExecuteAsync(int usuarioLoginId, string nuevoPasswordHash)
        {
            var userResult = await _loginRepository.GetByIdAsync(usuarioLoginId);
            if (userResult.IsFailure)
                return Result.Failure(userResult.ErrorCode ?? ErrorCodes.DbError, userResult.ErrorMessage ?? "Error al obtener usuario.");

            var user = userResult.Value;
            if (user == null)
                return Result.Failure(ErrorCodes.UsuarioLoginNotFound, "Usuario no encontrado.");

            user.CambiarPassword(nuevoPasswordHash);
            return await _loginRepository.UpdateAsync(user);
        }
    }
}
