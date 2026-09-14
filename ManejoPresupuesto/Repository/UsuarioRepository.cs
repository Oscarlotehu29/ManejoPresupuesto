namespace ManejoPresupuesto.Repository
{
    public interface IUsuarioRepository
    {
        public int ObtenerUsuarioId();
    }
    public class UsuarioRepository : IUsuarioRepository
    {
        public int ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
