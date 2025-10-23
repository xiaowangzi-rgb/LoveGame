using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSave {
    public class LUserDataEntity : ILGameData {
        public long id;
        public byte[] data;
        public long GetID() {
            return id;
        }
    }

    public interface ILGameData {
        long GetID();
    }
}
