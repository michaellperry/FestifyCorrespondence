using System.Threading.Tasks;

namespace Festify.Model
{
    public partial class Attendee
    {
        public Task<Slot> NewSlot(Time time)
        {
            return Community.AddFactAsync(new Slot(this, time));
        }
    }
}
