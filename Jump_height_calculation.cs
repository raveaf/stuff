using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Jump_height_calculation : MonoBehaviour {

    //the result
    public float jump_impulse;

    //desired jump height
    public float jump_height;
    
    public Rigidbody2D test_body;

    //checkbox to trigger the calculation once in play mode
    public bool do_calculation;

	void Update () {
		if (do_calculation && Application.isEditor) {            

            do_calculation = false;

            Physics2D.autoSimulation = false;

            Func<float,float> jump_height_function = (jump_impulse) => {
                float last_heigt = 0;
                
                test_body.simulated = true;
                test_body.velocity = Vector2.zero;
                test_body.position = Vector2.zero;

                test_body.AddForce(new Vector2(0, jump_impulse), ForceMode2D.Impulse );

                while (true) {
                    Physics2D.Simulate(Time.fixedDeltaTime, 1 << test_body.simulationGroup);

                    if (test_body.position.y <= last_heigt ) break;

                    last_heigt = test_body.position.y;
                }
                
                test_body.simulated = false;
            
                return last_heigt;
            };            

            jump_impulse = calculate_value(jump_height_function, jump_height, 0.0001f, 3, 150 );            

            Physics2D.autoSimulation = true;            
        }
	}

    //finds the right parameter value for the given function to return the target_value (within max_deviation) 
    //by trying out values between min_value and max_value
    //returns min_value or max_value if no such parameter value could be found
    private float calculate_value (Func<float,float> function, float target_value, float max_deviation, float min_value, float max_value) {
            
        float current_result = function(min_value);
        bool inverse_correlation = function(max_value) < current_result;            
                        
        float current_value;

        do {
            //using the value exactly between min and max
            current_value = (max_value + min_value) / 2;
            current_result = function(current_value);

            if (current_result == target_value || current_result == min_value || current_result == max_value) break;

            //updating min or max depending on the last result
            if (current_result > target_value) {
                if (inverse_correlation) {
                    min_value = current_value;
                } else {                        
                    max_value = current_value;
                }
            } else {
                if (inverse_correlation) {
                    max_value = current_value;
                } else {                        
                    min_value = current_value;
                }
            }

        } while (Mathf.Abs(target_value - current_result) > max_deviation );

        return current_value;
    }

}
