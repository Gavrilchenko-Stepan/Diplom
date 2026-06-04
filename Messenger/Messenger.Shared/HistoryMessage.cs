using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Shared
{
    [Serializable]
    public class HistoryMessage
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        public string ChatType { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Department { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
