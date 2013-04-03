namespace NugetSolutionValidator.Services
{
    public interface IBuilder<out T, in I>
    {
        T Build(I input);
    }
}