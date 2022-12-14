using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MBUretim.Mvc.Models
{

    // 0 (Başlamadı), 1 (Devam ediyor), 2 (Durduruldu), 3 (Tamamlandı), 4 (Kapandı) 
    public enum ChangePOAndWOStatus
    {
        NotStart = 0,
        Continue = 1,
        Stopped = 2,
        Completed = 3,
        Closed = 4
    }
    // 1 (üretim emri), 2 (iş emri) 
    public enum ChangePOAndWOStatusType
    {
        ProductOrder = 1,
        WorkOrder = 2
    }
    // True : Transaction kullanılsın, False : Transaction kullanılmasın
    public enum ChangePOAndWOStatusOpTrans
    {
        OpenTransaction = 0,
        DontTransaction = 1

    }
    // 0 (bağlantıları kopar), 1 (fişleri sil), 2 (silme)
    public enum ChangePOAndWOStatusDelStf
    {
        BreakConnection = 0,
        DeleteFiche = 1,
        DontDelete = 2

    }

    public enum ChangePOAndWOStatusError
    {
        NotStart = 100,
        Continue = 101,
        Stopped = 102,
        Completed = 103,
        Closed = 104,
        Slips=105
    }


}