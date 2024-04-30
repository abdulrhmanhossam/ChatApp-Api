namespace ChatApp.API.Extensions;
public static class DateTimeExtension
{
    public static int CalculateAge(this DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age)) age--;
        
        return age;
    }
}