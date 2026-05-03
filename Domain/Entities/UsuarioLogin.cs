using Taller_Mecanico_Users.Domain.ValueObjects;

namespace Taller_Mecanico_Users.Domain.Entities
{
    public class UsuarioLogin
    {
        public int UsuarioLoginId { get; set; }
        public int? EmpleadoId { get; private set; }
        public int? ClienteId { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public DateTime? UltimoAcceso { get; private set; }
        public bool Activo { get; private set; }
        public bool RequiereCambioPassword { get; private set; }
        public bool EsCliente { get; private set; }

        private UsuarioLogin() { }

        public static UsuarioLogin Crear(int empleadoId, string email, string passwordHash, bool requiereCambioPassword = true)
        {
            return new UsuarioLogin
            {
                EmpleadoId = empleadoId,
                Email = email,
                PasswordHash = passwordHash,
                Activo = true,
                RequiereCambioPassword = requiereCambioPassword,
                EsCliente = false
            };
        }

        public static UsuarioLogin Reconstituir(int usuarioLoginId, int? empleadoId, int? clienteId, string email, string passwordHash, DateTime? ultimoAcceso, bool activo, bool requiereCambioPassword = false, bool esCliente = false)
        {
            return new UsuarioLogin
            {
                UsuarioLoginId = usuarioLoginId,
                EmpleadoId = empleadoId,
                ClienteId = clienteId,
                Email = email,
                PasswordHash = passwordHash,
                UltimoAcceso = ultimoAcceso,
                Activo = activo,
                RequiereCambioPassword = requiereCambioPassword,
                EsCliente = esCliente
            };
        }

        public void RegistrarAcceso()
        {
            UltimoAcceso = DateTime.UtcNow;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public void Activar()
        {
            Activo = true;
        }

        public void CambiarPassword(string nuevoPasswordHash)
        {
            PasswordHash = nuevoPasswordHash;
            RequiereCambioPassword = false;
        }

        public void CambiarEmail(string nuevoEmail)
        {
            Email = nuevoEmail;
        }

        public void ResetearPassword(string nuevoPasswordHash)
        {
            PasswordHash = nuevoPasswordHash;
            RequiereCambioPassword = true;
        }

        public static UsuarioLogin CrearParaCliente(int clienteId, string email, string passwordHash)
        {
            return new UsuarioLogin
            {
                ClienteId = clienteId,
                Email = email,
                PasswordHash = passwordHash,
                Activo = true,
                RequiereCambioPassword = true,
                EsCliente = true
            };
        }
    }
}
