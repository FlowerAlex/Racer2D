using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject[] Road;
    public Sprite[] BrokenRoad;
    public Sprite[] AnotherCars;
    public GameObject Car;

    private System.Random rand;
    private List<GameObject> AnotherCarsOnTheRoad;
    private int timeWhenCarWasSpawned;
    private Image WarningImage;
    private GameObject BrokenRoadObject;


    void Start()
    {
        rand = new System.Random();
        timeWhenCarWasSpawned = 0;
        AnotherCarsOnTheRoad = new List<GameObject>();
        WarningImage = GameObject.Find("WarningImage").GetComponent<Image>();


    }



    void Update()
    {
        if (
            (int)Time.time % 4 == 0 &&
            (int)Time.time != timeWhenCarWasSpawned &&
            AnotherCarsOnTheRoad.Count < 4 &&
            (AnotherCarsOnTheRoad.Count == 0 ||
            AnotherCarsOnTheRoad.Max(Car => Car.transform.position.y) < Road.Max(road => road.transform.position.y) - 5f)
            )
        {
            if(rand.Next()%4 == 0 && BrokenRoadObject == null)
            {
                BrokenRoadObject = CreateBrokenRoad();
                BrokenRoadObject.transform.position = new Vector3(rand.Next(2) == 1 ? -0.7f : 0.7f, Road.Max(road => road.transform.position.y) + 10, -1);
                SetWarningImage(true);
            }
            else
            {
                AnotherCarsOnTheRoad.Add(SpawnSomeCar());
            }
            if(BrokenRoadObject != null && BrokenRoadObject.transform.position.y - 3 < Road.Min(road => road.transform.position.y))
            {
                Destroy(BrokenRoadObject);
                BrokenRoadObject = null;
            }
            if (BrokenRoadObject == null) SetWarningImage(false);
            timeWhenCarWasSpawned = (int)Time.time;
        }

    }

    private GameObject CreateBrokenRoad()
    {
        GameObject brokenRoad = new GameObject("BrokenRoad"), 
            barrierForBrokenRoad = new GameObject("barierForBrokenRoad"), 
            coneForBrokenRoad = new GameObject("ConeForBrokenRoad"),
            cone2ForBrokenRoad = new GameObject("Cone2ForBrokenRoad");
        brokenRoad.AddComponent<SpriteRenderer>().sprite = BrokenRoad[1];
        brokenRoad.AddComponent<BoxCollider2D>().size = new Vector2(1.7f,4.09f);
        //brokenRoad.GetComponent<BoxCollider2D>().isTrigger = true;

        barrierForBrokenRoad.AddComponent<SpriteRenderer>().sprite = BrokenRoad[0];
        barrierForBrokenRoad.transform.parent = brokenRoad.transform;
        barrierForBrokenRoad.transform.position = new Vector3(0,-2.5f,0);
        barrierForBrokenRoad.AddComponent<BoxCollider2D>().size = new Vector2(2f,0.2f);
        barrierForBrokenRoad.AddComponent<Rigidbody2D>().gravityScale = 0;
        coneForBrokenRoad.AddComponent<SpriteRenderer>().sprite = BrokenRoad[2];
        coneForBrokenRoad.transform.parent = brokenRoad.transform;
        coneForBrokenRoad.transform.position = new Vector3(-0.2f,-2.9f,0);
        coneForBrokenRoad.AddComponent<BoxCollider2D>().size = new Vector2(0.35f,0.35f);
        coneForBrokenRoad.AddComponent<Rigidbody2D>().gravityScale = 0;

        cone2ForBrokenRoad.AddComponent<SpriteRenderer>().sprite = BrokenRoad[2];
        cone2ForBrokenRoad.transform.parent = brokenRoad.transform;
        cone2ForBrokenRoad.transform.position = new Vector3(0.2f, -2.9f, 0);
        cone2ForBrokenRoad.AddComponent<BoxCollider2D>().size = new Vector2(0.35f, 0.35f);
        cone2ForBrokenRoad.AddComponent<Rigidbody2D>().gravityScale = 0;

        brokenRoad.transform.position = new Vector3(0,0,0);
        return brokenRoad;
    }

    private void FixedUpdate()
    {

        GameObject objectForDestroy = null, tmpObject;
        foreach (GameObject car in AnotherCarsOnTheRoad)
        {
            if ((tmpObject = AutonomousMovingCar(car)) != null)
            {
                objectForDestroy = tmpObject;
            }
        }
        if (objectForDestroy != null)
        {
            AnotherCarsOnTheRoad.Remove(objectForDestroy);
            Destroy(objectForDestroy);
        }
    }
    // return created car
    private GameObject SpawnSomeCar()
    {
        GameObject tmpCar = new GameObject("AnotherCar");
        tmpCar.AddComponent<SpriteRenderer>().sprite = AnotherCars[rand.Next(AnotherCars.Length)];
        tmpCar.AddComponent<CapsuleCollider2D>().size = new Vector2(3.72f, 9.83f);

        tmpCar.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        tmpCar.transform.position = new Vector3(rand.Next(2) == 1 ? -0.7f : 0.7f, Road.Max(road => road.transform.position.y) + 10, -1);
        tmpCar.SetActive(true);
        return tmpCar;
    }

    // return Car for destroy
    private GameObject AutonomousMovingCar(GameObject Car) {

        if (Car.transform.position.y > Road.Max(road => road.transform.position.y) + 15)
        {
            return Car;
        }
        else if (Car.transform.position.y < Road.Min(road => road.transform.position.y) - 5)
        {
            return Car;
        }
        else
        {
            Car.transform.Translate(0, (this.Car.GetComponent<CarController>().maxSpeed / 2f) *Time.deltaTime, 0);
        }
        return null;
    }

    private void SetWarningImage(bool visible)
    {
        if (visible)
        {
            WarningImage.enabled = true;
        }
        else
        {
            WarningImage.enabled = false;
        }
    }
}
