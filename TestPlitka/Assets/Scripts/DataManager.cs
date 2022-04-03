using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    const float WALL_W = 3f;
    const float WALL_H = 2f;
    [SerializeField] private GameObject _plitka; 
    [SerializeField] private GameObject _wall;
    [SerializeField] private GameObject _cont;
    [SerializeField] private GameObject _back;

    [SerializeField] InputField IF_angle;
    [SerializeField] InputField IF_step;
    [SerializeField] InputField IF_seem;
    [SerializeField] Text T_s;
    [SerializeField] InputField IF_tileX;
    [SerializeField] InputField IF_tileY;

    private int _max_x_objects;
    private int _max_y_objects;
    private Vector3 _plitka_bounds; // размеры плитки
    private Vector2 _startPoint; // стартовая точка (левый нижний угол)
    private float _seam; // шов
    private float _sPlitka; // полная площадь одной плитки
    private float _step; // смещение

    private float _angle; // угол поворота плитки
    private float _distX, _distY;
    private float _radiusWall;

    private float _radiusP;
    private float _rad;

    private Vector2 _pointWallInTile;
    private GameObject wallP;

    GameObject [,] _objects;
    
    void Start()
    {
       GetData();

      
    //  StartCoroutine(ICalculate());
    }

    public void ChangeData()
    {
        StartCoroutine(RestartScene());
    }
    void GetData()
    {
        _angle = float.Parse(IF_angle.text);
        _rad = (_angle -45) * Mathf.Deg2Rad;
        _seam = float.Parse(IF_seem.text)/1000;
        _step = float.Parse(IF_step.text)/1000;
        
        //_plitka_bounds = _plitka.transform.localScale;
        _plitka_bounds = new Vector3(float.Parse(IF_tileX.text)/1000, float.Parse(IF_tileY.text)/1000, 0.01f);
        if (_step >= _plitka_bounds.x) _step = Mathf.Repeat(_step , _plitka_bounds.x + _seam);
        _sPlitka = _plitka_bounds.x * _plitka_bounds.y;
        _radiusP = Mathf.Sqrt(Mathf.Pow(_plitka_bounds.x/2, 2) + Mathf.Pow(_plitka_bounds.y/2, 2));
        _radiusWall = Mathf.Sqrt(Mathf.Pow(WALL_W/2, 2) + Mathf.Pow(WALL_H/2, 2));
        float angle =  Mathf.Atan(WALL_H/WALL_W) * Mathf.Rad2Deg;
        _distX = _radiusWall * 2 * Mathf.Cos((angle - _angle) * Mathf.Deg2Rad);
        _distY = _radiusWall * 2 * Mathf.Sin((angle + _angle) * Mathf.Deg2Rad);      
        _startPoint = new Vector2(_distX / 2 - _plitka_bounds.x / 2 , _distY / 2 - _plitka_bounds.y / 2 );
        _max_x_objects = (int)(_distX / (_plitka_bounds.x + _seam) + 1);
        if (_step > 0) _max_x_objects ++;
        _max_y_objects = (int)(_distY / (_plitka_bounds.y + _seam) + 1);
        _objects = new GameObject [_max_x_objects, _max_y_objects];   

        StartCoroutine(ICalculate());
    }

    IEnumerator RestartScene()
    {
        yield return StartCoroutine(IDeleteScene());
        GetData();
    }
    IEnumerator IDeleteScene()
    {
        for (int j = 0; j < _max_y_objects; j++)
        {      
            for (int i = 0; i < _max_x_objects; i++)
            {   
                Destroy(_objects[i,j]) ;
            }
        }
        Destroy(wallP);
        yield return 0;
    }
    IEnumerator ICreateScene()
    {
        CreateWall();
        CreateObjects();
        SetAngel();
        CreateBack();
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ICalculate()
    {
        yield return StartCoroutine(ICreateScene());
        FullSquare();
    }
    void CreateWall()
    {
        wallP = Instantiate(_wall, _wall.transform.position, Quaternion.identity);
        wallP.transform.localScale = new Vector3(WALL_W, WALL_H, _wall.transform.localScale.z);

    }
    void CreateObjects()
    {
        Vector3 currentPoint;
        for (int j = 0; j < _max_y_objects; j++)
        {      
            for (int i = 0; i < _max_x_objects; i++)
            {   
                float _stepTemp ;
                _stepTemp = _step * j ;
               if ((j != 0) && (_step != 0))
               {
                    _stepTemp = Mathf.Repeat(_stepTemp , _plitka_bounds.x + _seam);
                    _stepTemp = _stepTemp - _plitka_bounds.x - _seam;
               }
               
                currentPoint = new Vector3(i*(_plitka_bounds.x + _seam) - _startPoint.x + _stepTemp , j*(_plitka_bounds.y + _seam) - _startPoint.y, 0);
                _objects[i,j] = Instantiate(_plitka, currentPoint, Quaternion.identity);
                _objects[i,j].transform.SetParent(_cont.transform);
                _objects[i,j].name = "tile" + i.ToString() + j.ToString();
                _objects[i,j].transform.localScale = new Vector3(float.Parse(IF_tileX.text)/2000, float.Parse(IF_tileY.text)/2000, 0.01f);
            }
        }
    }

    void SetAngel()
    {
        _cont.transform.Rotate(new Vector3(0,0, _angle));
    }

    void CreateBack()
    {
        GameObject temp;
        temp = Instantiate(_back, new Vector3(0,WALL_H/2+WALL_H/4, 0.1f),Quaternion.identity);// top
        temp.transform.localScale = new Vector3(WALL_W*2, WALL_H/2, 0.21f);
        temp = Instantiate(_back, new Vector3(0,-WALL_H/2-WALL_H/4, 0.1f),Quaternion.identity);// down
        temp.transform.localScale = new Vector3(WALL_W*2, WALL_H/2, 0.21f);
        temp = Instantiate(_back, new Vector3(-WALL_W, 0.1f),Quaternion.identity);// left
        temp.transform.localScale = new Vector3(WALL_W, WALL_H*2, 0.21f);
        temp = Instantiate(_back, new Vector3(WALL_W, 0.1f),Quaternion.identity);// rigth
        temp.transform.localScale = new Vector3(WALL_W, WALL_H*2, 0.21f);
    }

    void FullSquare()
    {
        string pointsNo;
        int pointYes;
        float Sfull = 0;
        int noPointX ;
        // определяем точки
        Vector2 [] points = new Vector2 [4];
        Vector2 centre;
        float [] tempX = new float[4]; // вершины плитки
        float [] tempY = new float[4]; // вершины плитки 
         for (int j = 0; j < _max_y_objects; j++)
        {      
            for (int i = 0; i < _max_x_objects; i++)
            {   
                centre.x = _objects[i,j].transform.position.x; // центр плитки (мир)
                centre.y = _objects[i,j].transform.position.y; // центр плитки (мир)
                int pointsInWall = 0;
                pointsNo = "";
                pointYes = 0;
                noPointX = 0;
                for (int c = 0; c < 4; c++)
                {
                  
                    points[c].x = _radiusP * Mathf.Cos((_angle -45 + 90 * c) * Mathf.Deg2Rad) + centre.x;
                    points[c].y = _radiusP * Mathf.Sin((_angle -45 + 90 * c) * Mathf.Deg2Rad) + centre.y;
                    if ((points[c].x >= -WALL_W/2)&&(points[c].x <= WALL_W/2)&&(points[c].y >= -WALL_H/2)&&(points[c].y <= WALL_H/2)) 
                    {
                        pointsInWall ++; 
                        pointYes = c;
                    }
                    else
                    {
                        pointsNo += c.ToString();

                        if (Mathf.Abs(points[c].x) < WALL_W/2 ) // находим самую близкую точку к стене (для case 0)
                        {
                            if (noPointX == 0)
                            {
                                noPointX = c;
                            }
                            else
                            {
                                if (points[noPointX].x > points[c].x) noPointX = c;
                            }

                        }
                    }
                }
                switch (pointsInWall)
                {
                    case 0: // + площадь угловой плитки с вершинами вне стены , но пересекает угол стороной
                        if (WallPointInTile(_objects[i,j]))
                        {
                            Debug.Log(_objects[i,j].transform.name);
                            float angleTemp = _angle;
                            float a = Mathf.Abs(points[noPointX].y) - WALL_H/2;
                            float b = WALL_W/2 - Mathf.Abs(points[noPointX].x);
                            if (_angle < 45) angleTemp = 90 - _angle;
                            float c = b - a * Mathf.Tan(angleTemp * Mathf.Deg2Rad);
                            float d = b * Mathf.Tan((90 - angleTemp) * Mathf.Deg2Rad) - a;
                            Sfull += (c * d) /2;
                            

                        }
                    break;
                    case 1: // + площадь плитки с одной вершиной на стене
                        if (_angle % 90 != 0)
                        {
                            if (WallPointInTile(_objects[i,j])) // если в углу стены плитка
                            {
                           
                                float a = WALL_W/2 - Mathf.Abs(points[pointYes].x);
                                float b = WALL_H/2 - Mathf.Abs(points[pointYes].y);
                                float s1 = a * a /2 * Mathf.Tan(_angle * Mathf.Deg2Rad);
                                float s2 = b * b /2 * Mathf.Tan(_angle * Mathf.Deg2Rad);
                                float c = Mathf.Abs(b - 2 * s1 / a);
                                Sfull += s1 +  s2 +  a * c;
                            }
                            else
                            {
                                if (Mathf.Abs(points[pointYes].x) > Mathf.Abs(points[pointYes].y)) // инверсия точки
                                {
                                float t = (WALL_W/2 - Mathf.Abs(points[pointYes].x)) * 2 + Mathf.Abs(points[pointYes].x);
                                if (points[pointYes].x < 0) t = -t;
                                points[pointYes].x = t;
                                } else
                                {
                                    float t = (WALL_H/2 - Mathf.Abs(points[pointYes].y)) * 2 + Mathf.Abs(points[pointYes].y);
                                    if (points[pointYes].y < 0) t = -t;
                                    points[pointYes].y = t;    
                                }
                               Sfull += STriangle(points[pointYes]);
                            }
                        }else
                        {
                             
                            Sfull += (Mathf.Abs(WALL_W/2) - Mathf.Abs(points[pointYes].x)) * (Mathf.Abs(WALL_H/2) - Mathf.Abs(points[pointYes].y));    
                        }

                    break;
                    case 2: // + площадь плитки с 2 вершинами на стене
                        int p1,p2;
                        p1 = int.Parse(pointsNo[0].ToString());
                        p2 = int.Parse(pointsNo[1].ToString());
                        if (!WallPointInTile(_objects[i,j])) 
                        {
                            if (_angle % 90 != 0)
                            {
                                Sfull += _sPlitka - Mathf.Abs(STriangle(points[int.Parse(pointsNo[0].ToString())]) - STriangle(points[int.Parse(pointsNo[1].ToString())]));
                            }
                            else
                            {
                                if (Mathf.Abs(points[p1].x) >= WALL_W/2)
                                {
                                    Sfull += _sPlitka - ((Mathf.Abs(points[p1].x) - WALL_W/2) * Vector2.Distance(points[p1],points[p2]));  
                                } else
                                {
                                    Sfull += _sPlitka - ((Mathf.Abs(points[p1].y) - WALL_H/2) * Vector2.Distance(points[p1],points[p2]));   
                                }
                            }
                        } else
                        {
                            
                            // если плитка в углу 
                            float a1 = WALL_W/2 - Mathf.Abs(points[p1].x);
                            float b1 = WALL_H/2 - Mathf.Abs(points[p1].y);
                            float a2 = WALL_W/2 - Mathf.Abs(points[p2].x);
                            float b2 = WALL_H/2 - Mathf.Abs(points[p2].y);
                            float s1 = a2 * a2 /2 * Mathf.Tan(_angle * Mathf.Deg2Rad);
                            float s2 = b1 * b1 /2 * Mathf.Tan(_angle * Mathf.Deg2Rad);
                            float b3 = Mathf.Abs(b2 - 2 * s1 / a2);
                            float s3 = b3 * a2;
                            float c = Vector2.Distance(points[p1], points[p2]);
                            float angleTemp = _angle;
                            if (_angle > 45) angleTemp = 90 - _angle;
                            float s4 = (c * c * Mathf.Sin((2 * angleTemp) * Mathf.Deg2Rad))/4;
                            Sfull += s1 +  s2 +  s3 + s4;
                            
                        }
                        
                        
                    break;
                    case 3:// + площадь без 1 вершины
                        Sfull += _sPlitka - STriangle(points[int.Parse(pointsNo)]);
                    break;

                    case 4: // + площадь полной плитки
                        Sfull += _sPlitka; 
                    break;
                }
            }
        }
        
        T_s.text = "Площадь: "+ Sfull.ToString("F2") + " кв.м.";
    }

   
   
    bool WallPointInTile(GameObject tile) // проверка нахождения плитки в углу стены
    {
        Vector2 [] points = new Vector2 [4];
        points[0] = new Vector2(WALL_W/2, WALL_H/2);
        points[1] = new Vector2(-WALL_W/2, WALL_H/2);
        points[2] = new Vector2(-WALL_W/2, -WALL_H/2);
        points[3] = new Vector2(WALL_W/2, -WALL_H/2);
        for (int c = 0; c < 4; c++)
                {
                  
                  
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(points[c].x, points[c].y,-1), new Vector3(0,0,10), out hit))
                    {
                        if (tile.transform.name == hit.transform.name)
                        {
                            _pointWallInTile = points[c];
                            return true;     
                        }
                    }
                }
        return false;
    }
    float STriangle(Vector2 p)
    {
        float h = 0;
        if ((p.x <= -WALL_W/2)||(p.x >= WALL_W/2)) h = Mathf.Abs(Mathf.Abs(p.x) - WALL_W/2);
        if ((p.y <= -WALL_H/2)||(p.y >= WALL_H/2)) h = Mathf.Abs(Mathf.Abs(p.y) - WALL_H/2);
        return h * h / 2 * Mathf.Tan((_angle%90)*Mathf.Deg2Rad) + h * h / (2 * Mathf.Tan((_angle%90)*Mathf.Deg2Rad));
    }

    void Update()
    {
        Debug.DrawLine(new Vector3(-1.5f,-1,-1), new Vector3(0,0,10),Color.green);
    }

    
}
