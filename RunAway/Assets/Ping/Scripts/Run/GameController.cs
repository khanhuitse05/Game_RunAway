﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameController : MonoBehaviour {
    public const int HeightGround = 50;
    static GameController _instance;
    public static GameController Instance
    {
        get { return _instance; }
    }
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float acceleration;
    [HideInInspector]
    public float maxSpeed;
    public Action OnGameOver;
    ObjectPoolManager poolManager;
    public GameObject pfGround;
    public Transform root;
    public HeroController hero;
    public Text textScore;
    float score = 0;
    public int Score
    {
        get { return (int)score; }
    }
    void Start () {
        _instance = this;
        Init();
	}
	void Init()
    {
        poolManager = new ObjectPoolManager();
        poolManager.Init(pfGround, root);
        poolManager.AddObject(5);
    }
    public void startGame()
    {
        Utils.Log("Start game");
        score = 0;
        textScore.text = "" + Score + " M";
        speed = GameConstants.Instance.speed;
        acceleration = GameConstants.Instance.acceleration;
        maxSpeed = GameConstants.Instance.maxSpeed;
        isGamePlay = true;
        hero.reset();
        hero.OnGhostDie = OnGhostDie;
        resetGround();
    }
    void resetGround()
    {
        poolManager.Reset();
        addFistGround();
    }
    // Update is called once per frame
    public bool isGamePlay;
    Ground _currentGround;
	void Update () {
        if (isGamePlay)
        {
            if (speed < maxSpeed)
            {
                speed += Time.deltaTime * acceleration;
            }
            score += speed * Time.deltaTime * 0.01f;
            textScore.text = "" + Score + " M";
            UpdateGround();
        }
	}
    void UpdateGround()
    {
        if (_currentGround.gameObject.transform.localPosition.x < 600)
        {
            addNextGround();
        }
    }
    void addFistGround()
    {
        _currentGround = poolManager.Spawn(1200, HeightGround, -570);
    }
    int _hh;
    int _xx;
    int _ww;
    int _minW;
    int _maxW;
    void addNextGround()
    {
        _hh = HeightGround + Utils.Random(-20, 100);
        int _jumpDistance = jumpDistace();
        int _distance = Utils.Random(200, _jumpDistance);
        if (_hh > _currentGround.height)
        {
            _distance -= _hh - _currentGround.height;
            _minW = _jumpDistance - _distance;
        }
        else
        {
            _minW = _jumpDistance - _distance + _currentGround.height - _hh;
        }

        if (_minW < 180)
        {
            _minW = 180;
        }
        _maxW = _jumpDistance - _distance + 600;
        _ww = Utils.Random(_minW, _maxW);
        _xx = (int)(_currentGround.width + _currentGround.transform.localPosition.x + _distance);
        _currentGround = poolManager.Spawn(_ww, _hh, _xx);
    }
    int jumpDistace()
    {
        return (int)(GameConstants.Instance.timeJump * speed);
    }
    float randomWidth()
    {
        return Utils.Random(1000, 10000);
    }
    public void DeActiveGround(Ground _object)
    {
        poolManager.Unspawn(_object);
    }
    public void OnGhostDie()
    {
        isGamePlay = false;
        if (OnGameOver != null)
            OnGameOver();
    }
}
