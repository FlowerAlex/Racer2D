using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{

    public float maxSpeed;
    public float maxRotateSpeed;
    public GameObject[] Road;
    public Camera Camera;
    public GameObject Car;

    public float actualSpeed;
    private float actualYPosLastRoad;
    private float actualYPosOfCar;
    private float actualRotateSpeed;
    private int roadCounter;
    private int store;
    private float maxYTranslateCamera;
    private float actualYTranslateCamera;


    private Vector3 VectorForCar;
    private GameObject RWall;
    private GameObject LWall;
    private GameObject BackWall;
    private Text ScoreText;
    public Image WarningImage;

    private static readonly float BONUS_MULTIPLIER_STORE = 2.5f;

    void Start()
    {
        actualSpeed = 0f;
        actualYPosLastRoad = 20;
        roadCounter = 0;
        RWall = GameObject.Find("RWall");
        LWall = GameObject.Find("LWall");
        BackWall = GameObject.Find("BackWall");
        WarningImage = GameObject.Find("WarningImage").GetComponent<Image>();
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        VectorForCar = new Vector3(0, 0, 0);
        maxSpeed = maxSpeed != 0 ? maxSpeed : 10;
        maxRotateSpeed = maxRotateSpeed != 0 ? maxRotateSpeed : 50f;
        actualRotateSpeed = 0f;
        store = 0;
        actualYTranslateCamera = 0;
        maxYTranslateCamera = 1.5f;
    }

    void FixedUpdate()
    {
        MoveCamera();
        FetchMap();
        MoveCar();
        CountStore();
        //SetWarningImage(false);
    }
    // Now this function works, but have't needed yet.
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

    private void CountStore()
    {
        if (actualYPosOfCar + 2 < Car.transform.position.y)
        {
            if (actualSpeed > maxSpeed / 1.5f)
            {
                store += (int)((Car.transform.position.y - actualSpeed) * BONUS_MULTIPLIER_STORE / 10);
            }
            else
            {
                store += (int)(Car.transform.position.y - actualSpeed) / 10;
            }
            ChangeTextOfStore();
            actualYPosOfCar = Car.transform.position.y;
        }
    }

    private void ChangeTextOfStore()
    {
        ScoreText.text = $"Score: {store}";
    }

    private void FetchMap()
    {
        if (Camera.transform.position.y > actualYPosLastRoad - 5)
        {
            actualYPosLastRoad += 10;
            Road[roadCounter].transform.Translate(0, 30, 0);
            BackWall.transform.Translate(0, 10, 0);
            roadCounter = (roadCounter + 1) % Road.Length;
        }
        MoveWalls();
    }

    private void MoveCamera()
    {
        if (actualSpeed > maxSpeed / 1.5f)
        {
            if (Camera.orthographicSize + Time.deltaTime < 8.5f)
            {
                Camera.orthographicSize += Time.deltaTime;
            }
            else
            {
                Camera.orthographicSize = 8.5f;
            }
        }
        else
        {
            if (Camera.orthographicSize - Time.deltaTime > 7f)
            {
                Camera.orthographicSize -= Time.deltaTime;
            }
            else
            {
                Camera.orthographicSize = 7f;
            }
        }
        if (actualSpeed > maxSpeed / 1.5f)
        {
            actualYTranslateCamera = actualYTranslateCamera + 2 * Time.deltaTime < maxYTranslateCamera ? actualYTranslateCamera + 2 * Time.deltaTime : maxYTranslateCamera;
        }
        else
        {
            actualYTranslateCamera = actualYTranslateCamera - 2 * Time.deltaTime > 0 ? actualYTranslateCamera - 2 * Time.deltaTime : 0;
        }
        if (Car.transform.position.y + 2 > actualYPosOfCar)
        {
            Camera.transform.position = new Vector3(Camera.transform.position.x, Car.transform.position.y + 4f + actualYTranslateCamera, Camera.transform.position.z);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (actualSpeed > 0)
        {
            if (actualSpeed > maxSpeed / 2)
            {
                actualSpeed /= 2;
            }
            else
            {
                actualSpeed = actualSpeed - (actualSpeed * 0.1f) > 0 ? actualSpeed - (actualSpeed * 0.1f) : 0;
            }
        }
    }

    private void MoveWalls()
    {
        LWall.transform.position = new Vector3(LWall.transform.position.x, Car.transform.position.y, LWall.transform.position.z);
        RWall.transform.position = new Vector3(RWall.transform.position.x, Car.transform.position.y, RWall.transform.position.z);
        BackWall.transform.position = new Vector3(BackWall.transform.position.x, actualYPosOfCar - 5.5f, BackWall.transform.position.z);
    }

    private void MoveCar()
    {
        ChangeSpeedCar();
        RotateCar();

        VectorForCar.Set(0, actualSpeed, 0);
        Car.transform.Translate(VectorForCar * Time.deltaTime);
    }

    private void ChangeSpeedCar()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (actualSpeed >= -maxSpeed / 2 && actualSpeed < maxSpeed)
            {
                if (actualSpeed < maxSpeed / 2)
                {
                    actualSpeed += maxSpeed * 0.05f;
                }
                else
                {
                    if (maxSpeed * 0.1f + actualSpeed <= maxSpeed)
                    {
                        actualSpeed += maxSpeed * 0.005f;
                    }
                    else
                    {
                        actualSpeed = maxSpeed;
                    }
                }
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (actualSpeed > 0)
            {
                actualSpeed -= maxSpeed * 0.05f;
            }
            else if (actualSpeed > -maxSpeed / 2)
            {
                if (actualSpeed - 0.15 * maxSpeed / 2 > 0)
                {
                    actualSpeed -= 0.15f * maxSpeed / 2;
                }
                else
                {
                    if (actualSpeed - 0.05f * (maxSpeed / 2) > -maxSpeed / 2)
                    {
                        actualSpeed -= 0.05f * (maxSpeed / 2);
                    }
                    else actualSpeed = -maxSpeed / 2;
                }
            }
        }
        else
        {
            SlowingCar();
        }
    }
    private void RotateCar()
    {
        if (Input.GetKey(KeyCode.A) && actualSpeed != 0)
        {
            if (actualSpeed > 0)
            {
                actualRotateSpeed = actualRotateSpeed + 0.2f * maxRotateSpeed < maxRotateSpeed ? actualRotateSpeed + 0.2f * maxRotateSpeed : maxRotateSpeed;
            }
            else
            {
                actualRotateSpeed = actualRotateSpeed - 0.2f * maxRotateSpeed > -maxRotateSpeed ? actualRotateSpeed - 0.2f * maxRotateSpeed : -maxRotateSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.D) && actualSpeed != 0)
        {
            if (actualSpeed > 0)
            {
                actualRotateSpeed = actualRotateSpeed - 0.2f * maxRotateSpeed > -maxRotateSpeed ? actualRotateSpeed - 0.2f * maxRotateSpeed : -maxRotateSpeed;
            }
            else
            {
                actualRotateSpeed = actualRotateSpeed + 0.2f * maxRotateSpeed < maxRotateSpeed ? actualRotateSpeed + 0.2f * maxRotateSpeed : maxRotateSpeed;
            }
        }
        else
        {
            NormalizeRotation();
        }
        Car.transform.Rotate(0, 0, actualRotateSpeed * Time.deltaTime);
    }
    private void NormalizeRotation()
    {
        if (actualRotateSpeed > 0.05 * maxRotateSpeed)
        {
            if (actualRotateSpeed - (maxRotateSpeed * 0.05f) > 0)
            {
                actualRotateSpeed -= maxRotateSpeed * 0.05f;
            }
            else actualRotateSpeed = 0;
        }
        else if (actualRotateSpeed < -0.05 * maxRotateSpeed)
        {
            if (actualRotateSpeed + (maxRotateSpeed * 0.05f) < 0)
            {
                actualRotateSpeed += maxRotateSpeed * 0.05f;
            }
            else actualRotateSpeed = 0;

        }
        else
        {
            actualRotateSpeed = 0;
        }
    }
    private void SlowingCar()
    {
        if (actualSpeed > 0)
        {
            actualSpeed = actualSpeed - maxSpeed * 0.005f > 0 ? actualSpeed - maxSpeed * 0.005f : 0;
        }
        else
        {
            actualSpeed = actualSpeed + maxSpeed * 0.02f < 0 ? actualSpeed + maxSpeed * 0.02f : 0;
        }
    }
}
