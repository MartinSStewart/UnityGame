using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets
{
    public class Logic : MonoBehaviour
    {
        Controller Controller;

        void Start()
        {
            try
            {
                Controller = new Controller();
            }
            catch(Exception e)
            {
                Debug.Break();
                throw e;
            }
        }

        void Update()
        {
            try
            {
                Controller.Update();
            }
            catch(Exception e)
            {
                Debug.Break();
                throw e;
            }
        }
    }
}

