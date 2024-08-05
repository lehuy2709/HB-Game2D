using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISate 
{
    void OnEnter(Enemy enemy);
    void OnExecute(Enemy enemy);
    void OnExit(Enemy enemy);

}
