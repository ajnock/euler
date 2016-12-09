using System.Threading.Tasks;

namespace Ulam
{
    interface ISpiral
    {
        /// <summary>
        /// Generates a Ulam spiral and persists it to somewhere
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task GenerateAndSave(string file = null);
    }
}
