using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedLibary {
    public interface IWObject {
        GameObject gameObject { get; set; }

        void Destroy();
    }
}


