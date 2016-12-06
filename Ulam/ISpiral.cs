using System.Threading.Tasks;

namespace Ulam
{
    interface ISpiral
    {
        Task GenerateAndSave(string file = null);
    }
}
