using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAbility : MonoBehaviour {

    //public GameObject effectVolume;
    //public Vector2 Range;
    //public bool requireTarget; 
    //public float manaCost; 
    public event EventHandler<EventArgs> StateChanged;


    [SerializeField] protected float channelTime = 0f;
    [SerializeField] protected float executeTime = 0f;
    [SerializeField] protected float cooldownTime = 0f;
    [SerializeField] private float timer = -1f;

    public enum AbilityState { Ready, Channeling, Executing, OnCooldown };

    [SerializeField] private AbilityState state = AbilityState.Ready;
    public AbilityState State
    {
        get { return state; }
    }

    public UnitModel MyUser
    {
        get;
        set;
    }

    public float TimeUntillStateChange
    {
        get
        {
            return timer;
        }
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer = Mathf.MoveTowards(timer, 0f, Time.deltaTime);
        }

        if (timer <= 0f && State != AbilityState.Ready)
        {
            switch (State)
            {
                case AbilityState.Channeling:
                    Execute();
                    ChangeState(AbilityState.Executing, executeTime);
                    break;

                case AbilityState.Executing:
                    StartCooldown();
                    ChangeState(AbilityState.OnCooldown, cooldownTime);
                    break;

                case AbilityState.OnCooldown:
                    SetReady();
                    ChangeState(AbilityState.Ready, -1);
                    break;
            }
        }
    }

    public bool Use()
    {
        if (MyUser == null)
            return false;

        if (CanUse())
        {
            bool used = DoUse();
            if (used)
            {
                StartChanneling();
                ChangeState(AbilityState.Channeling, channelTime);
            }
            return used;
        }

        return false;
    }

    public abstract bool CanUse();

    private void ChangeState(AbilityState newState, float stateDuration)
    {
        state = newState;
        timer = stateDuration;
        var handler = StateChanged;
        if (handler != null)
            handler(this, new EventArgs());
    }

    protected abstract bool DoUse();
    protected abstract bool StartChanneling();
    protected abstract bool Execute();
    protected abstract bool StartCooldown();
    protected abstract bool SetReady();
}
