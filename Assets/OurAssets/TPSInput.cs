using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSInput : Singleton<TPSInput> {


    public KeyCode JumpKey;
    public KeyCode InteractionKey;
    public KeyCode CrouchKey;
	public KeyCode RunKey;
	public KeyCode AimKey;
	public KeyCode RollKey;

	public Action onInteraction, onRoll, onJump;
	public Action<bool> onCrouchChanged, onRunningChanged, onAimingChanged;
    public Action<float> onHorizontalChanged, onVerticalChanged;

    private bool crouch = false;
    private bool Crouch
    {
        get
        {
            return crouch;
        }
        set
        {
            if (crouch!=value)
            {
                crouch = value;
                if (onCrouchChanged!=null)
                {
                    onCrouchChanged.Invoke(crouch);
                }
            }
        }
    }

	private bool aiming = false;
	private bool Aiming
	{
		get
		{
			return aiming;
		}
		set
		{
			if (aiming!=value)
			{
				aiming = value;
				if (onAimingChanged!=null)
				{
					onAimingChanged.Invoke(aiming);
				}
			}
		}
	}

    private float horizontal = 0;
    private float Horizontal
    {
        get
        {
            return horizontal;
        }
        set
        {
            if (horizontal != value)
            {
                horizontal = value;
                if (onHorizontalChanged != null)
                {
                    onHorizontalChanged.Invoke(horizontal);
                }
            }
        }
    }

    private float vertical = 0;
    private float Vertical
    {
        get
        {
            return vertical;
        }
        set
        {
            if (vertical != value)
            {
                vertical = value;
                if (onVerticalChanged != null)
                {
                    onVerticalChanged.Invoke(vertical);
                }
            }
        }
    }

	private bool runing;
	public bool Runing
	{
		get
		{
			return runing;
		}
		set
		{
			if (runing!=value)
			{
				runing = value;
				if (onRunningChanged!=null)
				{
					onRunningChanged.Invoke(runing);
				}
			}
		}
	}

    void Update () {
        if (Input.GetKeyDown(InteractionKey))
        {
            if (onInteraction != null)
            {
                onInteraction.Invoke();
            }
        }

		if (Input.GetKeyDown(JumpKey))
		{
			if (onJump != null)
			{
				onJump.Invoke();
			}
		}

		if (Input.GetKeyDown(RollKey))
		{
			if (onRoll != null)
			{
				onRoll.Invoke();
			}
		}

        Crouch = Input.GetKeyDown(CrouchKey);
		Runing = Input.GetKeyDown (RunKey);
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
		Aiming = Input.GetKeyDown(JumpKey);
    }
}
