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

    // �ڷ�ƾ ���� �� Coroutine ��ü ��ȯ
    public static Coroutine StartManagedCoroutine(IEnumerator coroutine, string name)
    {
        if (activeCoroutines.ContainsKey(name))
        {
            Debug.LogWarning($"Coroutine with name {name} is already running.");
            return null; // �̹� ���� ���� ���, null ��ȯ
        }

        // StartCoroutine�� ȣ���ϰ� ��ȯ�� Coroutine ��ü�� CoroutineInfo�� �Բ� ����
        Coroutine co = _instance.StartCoroutine(coroutine);
        CoroutineInfo info = new CoroutineInfo(coroutine, CoroutineState.Running, co);
        activeCoroutines[name] = info;

        return co; // ���۵� Coroutine ��ü ��ȯ
    }

    // �ڷ�ƾ ���� �� ����
    private IEnumerator RunCoroutine(CoroutineInfo info, string name)
    {
        yield return _instance.StartCoroutine(info.Coroutine);
        activeCoroutines.Remove(name);
    }

    // �ڷ�ƾ�� �Ͻ� ���� ���� Ȯ��
    public static bool IsCoroutinePaused(string name)
    {
        if (activeCoroutines.TryGetValue(name, out CoroutineInfo info))
        {
            return info.State == CoroutineState.Paused;
        }
        return false;
    }

    // �ڷ�ƾ �Ͻ� ����
    public static void PauseManagedCoroutine(string name)
    {
        if (activeCoroutines.TryGetValue(name, out CoroutineInfo info))
        {
            info.State = CoroutineState.Paused;
            // ���⼭ ���� �ڷ�ƾ�� �Ͻ������ϴ� ������ Unity�� �ڷ�ƾ �ý������δ� ���� ������ �� �����ϴ�.
            // ��� �ڷ�ƾ ���� ���� ������ �� ���¸� üũ�ؾ� �մϴ�.
        }
    }

    // �ڷ�ƾ �簳
    public static void ResumeManagedCoroutine(string name)
    {
        if (activeCoroutines.TryGetValue(name, out CoroutineInfo info))
        {
            info.State = CoroutineState.Running;
            // �ڷ�ƾ�� '�簳'�ϴ� ���� ���� �ڷ�ƾ ���� ���� ������ ���¸� üũ�Ͽ� �����մϴ�.
        }
    }

    // �ڷ�ƾ ����
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
        // Coroutine ��ü�� activeCoroutines���� �ش� �׸��� ã���ϴ�.
        string keyToRemove = null;
        foreach (var pair in activeCoroutines)
        {
            if (pair.Value.CoroutineObject == co)
            {
                // ��ġ�ϴ� Coroutine ��ü�� ã�ҽ��ϴ�. �ڷ�ƾ�� ����ϴ�.
                _instance.StopCoroutine(co);
                keyToRemove = pair.Key;
                break;
            }
        }

        if (keyToRemove != null)
        {
            // ��ųʸ����� �ش� �ڷ�ƾ ������ �����մϴ�.
            activeCoroutines.Remove(keyToRemove);
        }
    }


    // CoroutineManager�� �߰�
    public static List<string> GetRunningCoroutineNames()
    {
        return new List<string>(activeCoroutines.Keys);
    }

    // ���� ���� ���� �ڷ�ƾ�� ���� ���� ��ȯ
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