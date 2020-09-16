using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NixieArrayScript : MonoBehaviour
{
    [SerializeField]
    private NixieScript nixieScript0, nixieScript1, nixieScript2, nixieScript3, nixieScript4,
    nixieScript5, nixieScript6, nixieScript7, nixieScript8, nixieScript9;

    private NixieScript[] nixies;
    BCD10 lastDisplayed;
    // Start is called before the first frame update
    void Start()
    {
        nixies = new NixieScript[10];
        nixies[0] = nixieScript0;
        nixies[1] = nixieScript1;
        nixies[2] = nixieScript2;
        nixies[3] = nixieScript3;
        nixies[4] = nixieScript4;
        nixies[5] = nixieScript5;
        nixies[6] = nixieScript6;
        nixies[7] = nixieScript7;
        nixies[8] = nixieScript8;
        nixies[9] = nixieScript9;
        lastDisplayed = new BCD10();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNumber(BCD10 b)
    {
        if (!b.isEqual(lastDisplayed))
        {
            for (int i = 0; i < 10; i++)
            {
                nixies[9 - i].setNumber(b.digits[i]);
            }
        }
        lastDisplayed.set(b);
    }

}
