using Flunt.Notifications;
using Flunt.Validations;

namespace SistemaCadastro.ViewModel;

public class PontoViewModel : Notifiable<Notification>
{
    public Guid FuncionarioId { get; set; }
    public DateTime Data { get; set; }

    public Ponto MapTo()
    {
        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsNotNull(Data, "Informe a data do registro.")
        );

        return new Ponto(Guid.NewGuid(), Data, FuncionarioId);
    }
}

