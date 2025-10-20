using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IObject
{
    private PlayerObjNavMesh _nowObj;
    private Camera _camera;
    private Room _currentRoom;

    [Header("相机X轴限制")]
    public Vector2 CameraClampX;

    [Header("相机Y轴限制")]
    public Vector2 CameraClampY;

    [Header("是否相机跟随")]
    public bool isCameraFollow = true;

    [Header("是否可以移动")]
    public bool isCanMove = true;

    public enum PlayerEventParam {
        EnterRoom,
    }

    private void Awake()
    {
        //注册事件
        GloabEventManager.Instance.AddListener<PlayerEventData>(EventType.Player.ToString(),OnTriggerEvent);
    }

    private void Start()
    {
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _nowObj = GetComponent<PlayerObjNavMesh>();
        InitState();
    }

    void LateUpdate() {
        CameraFollow();
    }

    void Update() {
        MovePlayer();
    }

    /// <summary>
    /// 初始化状态
    /// </summary>
    private void InitState() {
        transform.position = PointGroupTools.GetPointData(PointType.StartPoint).position;
    }

    /// <summary>
    /// 移动玩家
    /// </summary>
    private void MovePlayer() {
        if (!isCanMove) {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null){
                //Set move Player object to this point#
                if (_nowObj != null)
                {
                    Vector2 goalPos = hit.point;
                    _nowObj.SetMovePos(goalPos);
                }
            }
        }
    }

    /// <summary>
    /// 相机跟随
    /// </summary>
    private void CameraFollow(){
        if (!isCameraFollow) {
            return;
        }
        if (_camera == null) {
            return;
        }
        if (_nowObj == null) {
            return;
        }
        Vector3 pos = _camera.transform.position;
        pos.x = _nowObj.transform.position.x;
        pos.y = _nowObj.transform.position.y;
        pos.x = Mathf.Clamp(pos.x, CameraClampX.x, CameraClampX.y);
        pos.y = Mathf.Clamp(pos.y, CameraClampY.x, CameraClampY.y);
        _camera.transform.position = pos;
    }

    /// <summary>
    /// 玩家说话
    /// </summary>
    /// <param name="text"></param>
    public void PlayerSpeak(string text) {
        Tools.DoSpeak(transform, text, 1f, 1.0f, null);
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventParam"></param>
    public void OnTriggerEvent(PlayerEventData eventParam){
        if (eventParam == null) {
            return;
        }
        //进入房间
        if (eventParam.eventParam == PlayerEventParam.EnterRoom.ToString()) {
            _nowObj?.ResetMove();
        }
    }

    public Room GetRoom() {
        return _currentRoom;
    }

    /// <summary>
    /// 创建玩家
    /// </summary>
    /// <returns></returns>
    public static PlayerController CreatePlayer() {
        var player = Resources.Load<GameObject>("Players/Player");
        if (player == null) {
            return null;
        }
        var playerObj = GameObject.Instantiate(player);
        if (playerObj == null) {
            return null;
        }
        return playerObj.GetComponent<PlayerController>();
    }
}
