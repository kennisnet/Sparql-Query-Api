using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trezorix.Sparql.Api.Core {
  using MongoDB.Bson;

  public static class ObjectIdExtension {
    public static Guid AsGuid(this ObjectId oid) {
      var bytes = oid.ToByteArray().Concat(new byte[] { 5, 5, 5, 5 }).ToArray();
      Guid gid = new Guid(bytes);
      return gid;
    }

    public static ObjectId AsObjectId(this Guid gid) {
      var bytes = gid.ToByteArray().Take(12).ToArray();
      var oid = new ObjectId(bytes);
      return oid;
    }
    
    public static bool IsObjectId(this string id) {
      ObjectId objectId;
      return ObjectId.TryParse(id, out objectId);
    }
  }
}
