using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class Controller
    {
        Player player;

        public Controller()
        {
            var cube = GameObject.FindGameObjectWithTag("Level");
            var viewedModel = (MeshFilter)cube.GetComponent(typeof(MeshFilter));

            var coord = new SurfaceCoord(new ReadOnlyMesh(viewedModel.mesh), 0, new Vector2(0.2123f, 0.45123f));

            player = new Player
            {
                Position = coord
            };

            var playerModel = GameObject.FindGameObjectWithTag("Player");
            playerModel.transform.SetParent(viewedModel.transform);
            playerModel.transform.localPosition = new UnityEngine.Vector3();
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                player.Position = player.Position.Move(MathExt.VectorFromAngle(player.Position.Rotation, 0.01));
            }
            var pos = player.Position;
            if (Input.GetKey(KeyCode.A))
            {
                player.Position = player.Position.Rotate(-0.02f);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                player.Position = player.Position.Rotate(0.02f);
            }

            var playerModel = GameObject.FindGameObjectWithTag("Player");
            playerModel.transform.position = new UnityEngine.Vector3();
            playerModel.transform.localPosition = player.Position.GetLocalCoord().ToUnity();
        }
    }
}
