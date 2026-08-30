namespace KurumiConcursos.Domain.Extensions;

public static class DateTimeExtension
{
    public static DateTimeOffset GetDateAndTimeInBrasilia(this DateTime date) =>
        date.ToUniversalTime().AddHours(-3);

    public static DateTime GetDate(this DateTime? date) =>
        date.GetValueOrDefault().Date;

    public static int CalculateAge(this DateTime birthDate)
    {
        var currentDate = DateTime.Now.GetDateAndTimeInBrasilia();
        var age = currentDate.Year - birthDate.Year;

        if (birthDate > currentDate.AddYears(-age))
            age--;

        return age;
    }
}