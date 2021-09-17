using System.Collections.Generic;

namespace todo_webApi.Utils.Models
{
   public class GetAllResponseBody<T>
   {
      public GetAllResponseBody(List<T> itemList, int total)
      {
         ItemList = itemList;
         Total = total;
      }

      public List<T> ItemList { get; }
      public int Total { get; }
   }
}
