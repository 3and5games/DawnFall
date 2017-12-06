using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSInput : Singleton<TPSInput> {


    public KeyCode JumpKey;
    public KeyCode InteractionKey;
    public KeyCode CrouchKey;

    public Action onJump, onInteraction;
    public Action<bool> onCrouchChanged;
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

    void Update () {
        if (Input.GetKeyDown(JumpKey))
        {
            if (onJump!=null)
            {
                onJump.Invoke();
            }
        }
        if (Input.GetKeyDown(InteractionKey))
        {
            if (onInteraction != null)
            {
                onInteraction.Invoke();
            }
        }

        Crouch = Input.GetKeyDown(CrouchKey);

        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");

    }
}
