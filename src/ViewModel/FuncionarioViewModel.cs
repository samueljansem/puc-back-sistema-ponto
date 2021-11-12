using Flunt.Notifications;
using Flunt.Validations;

namespace SistemaCadastro.ViewModel;

public class FuncionarioViewModel : Notifiable<Notification>
{
    public string Nome { get; set; }
    public string Matricula { get; set; }
    public string Senha { get; set; }

    public Funcionario MapTo()
    {
        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsNotNull(Nome, "Informe o nome do funcionário.")
        );

        Random generator = new Random();
        String matricula = generator.Next(0, 1000000).ToString("D6");

        return new Funcionario { Id = Guid.NewGuid(), Nome = Nome, Matricula = matricula, Senha = Senha };
    }
}

