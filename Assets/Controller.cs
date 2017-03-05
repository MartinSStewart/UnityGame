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
            //Plane plane = new Plane(new Vector3(), new Vector3(), null);
            var cube = GameObject.FindGameObjectWithTag("Level");
            var viewedModel = (MeshFilter)cube.GetComponent(typeof(MeshFilter));

            var coord = new SurfaceCoord(new ReadonlyMesh(viewedModel.mesh), 0, new Vector2(0.2123f, 0.45123f));

            player = new Player
            {
                Position = coord
            };

            var playerModel = GameObject.FindGameObjectWithTag("Player");
            playerModel.transform.SetParent(viewedModel.transform);
            playerModel.transform.localPosition = new Vector3();
            //player.transform.position = coord.GetWorldCoord();
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
                player.Position = player.Position.Rotate(-2f);//new SurfaceCoord(pos.Mesh, pos.TriangleIndex, pos.Coord, pos.Rotation - 0.1f, pos.FrontSide);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                player.Position = player.Position.Rotate(2f); //new SurfaceCoord(pos.Mesh, pos.TriangleIndex, pos.Coord, pos.Rotation + 0.1f, pos.FrontSide);
            }

            var playerModel = GameObject.FindGameObjectWithTag("Player");
            playerModel.transform.position = new Vector3();
            playerModel.transform.localPosition = player.Position.GetLocalCoord();
        }
    }
}
