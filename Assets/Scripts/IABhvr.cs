using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IABhvr : MonoBehaviour
{
    #region Variaveis
    public GameObject target;
    public NavMeshAgent agent;
    public Vector3 patrolposition;
    public float patrolDistance = 10;
    public float stoppedTime;
    public float distancetotrigger = 10;
    public float distancetoattack = 3;
    public float timetowait = 3;
    public float energia = 30;
    public Transform restPos;
    #endregion
    //Estados possiveis para a FSM
    public enum States
    {
        pursuit,
        attacking,    
        patrol,    
        rest, 
    }

    public States state;

    // Start is called before the first frame update
    void Start()
    {
        patrolposition = new Vector3(transform.position.x + Random.Range(-patrolDistance, patrolDistance), transform.position.y, transform.position.z + Random.Range(-patrolDistance, patrolDistance));
    }

    private void Update()
    {
        StateMachine();
    }

    //Maquina de estados que troca o comportamento da IA dependendo de certas condicoes
    void StateMachine()
    {
        switch (state)
        {
            case States.pursuit:
                PursuitState();
                break;
            case States.attacking:
                AttackState();
                break;
            case States.patrol:
                PatrolState();
                break;
            case States.rest:
                RestState();
                break;
        }
    }

    void RestState()
    {
        agent.destination = restPos.position;
        
            if(energia < 30)
                energia += 2 * Time.deltaTime;
            if(energia >= 30)
                state = States.patrol;
        
    }
    //Persegue o player se estiver na distancia para perseguir, caso nao esteja, volta a patrulhar ou ataca se estiver proximo demais
    void PursuitState()
    {
        agent.isStopped = false;
        agent.destination = target.transform.position;
        energia -= Time.deltaTime;
        //if (Vector3.Distance(transform.position, target.transform.position) < 5)
        //{
        //    state = States.breath;
        //}
        if(energia > 0)
        {
            if (Vector3.Distance(transform.position, target.transform.position) >= distancetotrigger)
            {
                state = States.patrol;
            }           
            if(Vector3.Distance(transform.position,target.transform.position) <= distancetoattack)
            {
                state = States.attacking;
            }
        }
        else
        {
            state = States.rest;
        }
        
    }
    //Ataca o player se estiver na distancia para atacar, caso nao esteja volta a patrulhar
    void AttackState()
    {
        energia -= Time.deltaTime;
        agent.isStopped = true;
        print("Attacking player!");
        if(energia > 0)
        {
            if(Vector3.Distance(transform.position,target.transform.position) > distancetoattack)
            {
                state = States.patrol;
            }
        }
        else
        {
            state = States.rest;
        }
        
    }
    //Patrulha ate encontrar o jogador
    void PatrolState()
    {
        energia -= Time.deltaTime;
        agent.isStopped = false;
        agent.SetDestination(patrolposition);
        if(energia > 0)
        {
            //tempo parado 
            if (agent.velocity.magnitude < 0.1f)
            {
                stoppedTime += Time.deltaTime;
            }
            //se for mais q timetowait segundos
            if (stoppedTime > timetowait)
            {
                stoppedTime = 0;
                patrolposition = new Vector3(transform.position.x + Random.Range(-patrolDistance, patrolDistance), transform.position.y, transform.position.z + Random.Range(-patrolDistance, patrolDistance));
            }
            //distancia do jogador for menor q distancetotrigger
            if (Vector3.Distance(transform.position, target.transform.position) < distancetotrigger)
            {
                state = States.pursuit;
            }
        }  
        else
        {
            state = States.rest;
        }       
    }
}