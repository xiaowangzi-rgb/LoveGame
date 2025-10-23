
namespace UnityEngine.UI {
    /// <summary>
    /// 替代空image做点击
    /// </summary>
    public class Empty4Raycast : MaskableGraphic {

        protected Empty4Raycast() {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
        }
    }
}