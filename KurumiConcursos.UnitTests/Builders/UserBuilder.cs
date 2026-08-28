using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.UnitTests.Builders;

public sealed class UserBuilder
{
    string _name = "Pessoa Teste", _email = "pessoa@kurumi_concursos.test";

    public UserBuilder WithName(string value)
    {
        _name = value;
        return this;
    }

    public UserBuilder WithEmail(string value)
    {
        _email = value;
        return this;
    }

    public User Build() => new()
    {
        Id = Guid.NewGuid(),
        UserName = _email,
        Email = _email,
        PersonalData = new PersonalData { FullName = _name }
    };
}
