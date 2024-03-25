using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public enum CoroutineState { Running, Paused }

    private class CoroutineInfo
    {
        public IEnumerator Coroutine;
        public CoroutineState State;
        public Coroutine CoroutineObject;

        public CoroutineInfo(IEnumerator coroutine, CoroutineState state, Coroutine coroutineObject)
        {
            Coroutine = coroutine;
            State = state;
            CoroutineObject = coroutineObject;
        }
    }

    private static Dictionary<string, CoroutineInfo> activeCoroutines = new Dictionary<string, CoroutineInfo>();
    private static CoroutineManager _instance;
    public static CoroutineManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CoroutineManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("CoroutineManager");
                    _instance = obj.AddComponent<CoroutineManager>();
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // 코루틴 시작 및 Coroutine 객체 반환
    public static Coroutine StartManagedCoroutine(IEnumerator coroutine, string name)
    {
        if (activeCoroutines.ContainsKey(name))
        {
            Debug.LogWarning($"Coroutine with name {name} is already running.");
            return null; // 이미 실행 중인 경우, null 반환
        }

        // StartCoroutine을 호출하고 반환된 Coroutine 객체를 CoroutineInfo와 함께 저장
        Coroutine co = _instance.StartCoroutine(coroutine);
        CoroutineInfo info = new CoroutineInfo(coroutine, CoroutineState.Running, co);
        activeCoroutines[name] = info;

        return co; // 시작된 Coroutine 객체 반환
    }

    // 코루틴 실행 및 관리
    private IEnumerator RunCoroutine(CoroutineInfo info, string name)
    {
        yield return _instance.StartCoroutine(info.Coroutine);
        activeCoroutines.Remove(name);
    }

    // 코루틴의 일시 정지 상태 확인
    public static bool IsCoroutinePaused(string name)
    {
        if (activeCoroutines.TryGetValue(name, out CoroutineInfo info))
        {
            return info.State == CoroutineState.Paused;
        }
        return false;
    }

    // 코루틴 일시 정지
    public static void PauseManagedCoroutine(string name)
    {
        if (activeCoroutines.TryGetValue(name, out CoroutineInfo info))
        {
            info.State = CoroutineState.Paused;
            // 여기서 실제 코루틴을 일시정지하는 로직은 Unity의 코루틴 시스템으로는 직접 구현할 수 없습니다.
            // 대신 코루틴 실행 로직 내에서 이 상태를 체크해야 합니다.
        }
    }

    // 코루틴 재개
    public static void ResumeManagedCoroutine(string name)
    {
        if (activeCoroutines.TryGetValue(name, out CoroutineInfo info))
        {
            info.State = CoroutineState.Running;
            // 코루틴을 '재개'하는 로직 역시 코루틴 실행 로직 내에서 상태를 체크하여 구현합니다.
        }
    }

    // 코루틴 종료
    public static void StopManagedCoroutine(string name)
    {
        if (activeCoroutines.TryGetValue(name, out CoroutineInfo info))
        {
            _instance.StopCoroutine(info.Coroutine);
            activeCoroutines.Remove(name);
        }
    }


    public static void StopManagedCoroutine(Coroutine co)
    {
        // Coroutine 객체로 activeCoroutines에서 해당 항목을 찾습니다.
        string keyToRemove = null;
        foreach (var pair in activeCoroutines)
        {
            if (pair.Value.CoroutineObject == co)
            {
                // 일치하는 Coroutine 객체를 찾았습니다. 코루틴을 멈춥니다.
                _instance.StopCoroutine(co);
                keyToRemove = pair.Key;
                break;
            }
        }

        if (keyToRemove != null)
        {
            // 딕셔너리에서 해당 코루틴 정보를 제거합니다.
            activeCoroutines.Remove(keyToRemove);
        }
    }


    // CoroutineManager에 추가
    public static List<string> GetRunningCoroutineNames()
    {
        return new List<string>(activeCoroutines.Keys);
    }

    // 현재 실행 중인 코루틴의 상태 정보 반환
    public static Dictionary<string, CoroutineState> GetRunningCoroutineStates()
    {
        Dictionary<string, CoroutineState> states = new Dictionary<string, CoroutineState>();
        foreach (var pair in activeCoroutines)
        {
            states.Add(pair.Key, pair.Value.State);
        }
        return states;
    }
}