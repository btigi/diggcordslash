using diggcordslash.Model;

namespace diggcordslash.Commands;

interface ICommand
{
    public Task<Interaction> Command(Interaction interaction, IServiceProvider serviceProvider);
}
