using System.Collections;
using UnityEngine;
using System.Collections.Generic;  // Add this at the top
using TMPro;  // For TextMeshPro support

// todo texte und zeiten

public class StepManager : MonoBehaviour
{
    public GameObject Laptop;  // Reference to the parent GameObject containing all the parts
    public GameObject Monitor;

    public GameObject Screws1;
    public GameObject Screws2;

    public GameObject ScrewDriver;
    private Vector3 screwdriverOffset = new Vector3(0, 0.118f, 0);  
    private Vector3 initialCasingDiskPosition = new Vector3(0,0,0);
    public GameObject BatteryOld;
    public GameObject BatteryNew;



    public GameObject CoverElectronics;
    public GameObject CasingDisk;

    public GameObject Arrow;

    public TextMeshPro infoText3D;  // Reference to the 3D TextMeshPro component
    public float delayBetweenSteps = 1f;  // Delay between each step in seconds
    public float rotationDuration = 1f;   // Duration for smooth rotation or movement

        private Vector3 storedVector = new Vector3(0, 0, 0);  // Klassenvariable
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();


    void Start()
    {
            Arrow.SetActive(false);

        StartCoroutine(PerformStepsSequence());
    }

    IEnumerator PerformStepsSequence()
    {
        // Step 1: Rotate the whole group smoothly
        UpdateInfoText("Step 1: Close Laptop and Remove all plugged in devices");
        yield return StartCoroutine(RotateMonitorSmooth(-90));  // Rotate by 90 degrees
        yield return new WaitForSeconds(delayBetweenSteps);

        // Step 2: Rotate the whole group again
        UpdateInfoText("Step 2: Turn Laptop upside down");
        // Rotieren um die y-Achse mit dem Monitor als Referenzpunkt
        Vector3 customAxis = new Vector3(0, 0, 1);  // Eine Achse in Richtung x-y
        yield return StartCoroutine(RotateGroupSmooth(180f, Monitor.transform, customAxis));
        yield return new WaitForSeconds(delayBetweenSteps);


        // Step : Move CasingElectronics to the left
        UpdateInfoText("Step 3: Take the screwdriver");
        yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(-0.0251f, 1.0214f, 0.6565f), new Vector3(90, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);


        // Step : Move CasingElectronics to the left
        UpdateInfoText("Step 4: Remove all visible Screws");
        Vector3 targetPositionScrews1 = new Vector3(-0.325f, 0.887f, 0.654f);  // Replace with the actual target position
        yield return StartCoroutine(RemoveScrews1(targetPositionScrews1, 0.1f));  // Move screws over 1 second
            yield return new WaitForSeconds(delayBetweenSteps);

        // yield return StartCoroutine(RemoveScrews1(new Vector3(-0.325f, 0.887f, 0.654f))); // xxx todo
        yield return new WaitForSeconds(delayBetweenSteps);
        yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(0.695f, 0.934f, 0.581f), new Vector3(0, 0, 0)));


        // Step : Move CasingDisk to the left and down
        UpdateInfoText("Step 4: remove CasingDisk");
                Vector3 targetPositionCasingDisk = new Vector3(-0.325f, 0.887f, 0.754f);  // Replace with the actual target position

        yield return StartCoroutine(MoveCasingDiskSmooth(targetPositionCasingDisk));


        // Step : Move CasingElectronics to the left
        UpdateInfoText("Step 3: Take the screwdriver");
         yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(-0.1606f, 1.0214f, 0.6565f), new Vector3(90, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);


                // Step : Move CasingElectronics to the left
        UpdateInfoText("Step 4: RemoveScrews2");


        Vector3 targetPositionScrews2 = new Vector3(-0.325f, 0.887f, 0.754f);  // Replace with the actual target position
        yield return StartCoroutine(RemoveScrews2(targetPositionScrews2, 0.1f));  // Move screws over 1 second



        // yield return StartCoroutine(RemoveScrews2(new Vector3(-1, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);
                yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(0.695f, 0.934f, 0.581f), new Vector3(0, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);


    // Step 5: Move CasingDisk and Show Arrow at (0.184, 0.792, 0.555)
    UpdateInfoText("Step 5: unplug cover electronics, start at green arrow");
    Arrow.transform.position = new Vector3(0.184f, 0.792f, 0.555f);  // Set arrow position
    Arrow.SetActive(true);  // Activate the arrow
        yield return new WaitForSeconds(delayBetweenSteps);
// Assuming the rectangular object has a length of 5 units and you want to rotate around one of the longer edges
Vector3 pivotOffset = new Vector3(0, 0, -0.4585f); // Adjust this to the correct edge
yield return StartCoroutine(RotateCoverElectronicsAroundEdge(-180f));  // Rotates 90 degrees around the calculated edge


        // yield return StartCoroutine(RotateCoverElectronics(-180f));
    yield return new WaitForSeconds(delayBetweenSteps);
    Arrow.SetActive(false);  // Deactivate the arrow after the step

    // Step 6: Unplug cover electronics and Show Arrow at (0, 0, 0)
    UpdateInfoText("Step 6: remove cable at green arrow, remove battery");
    Arrow.transform.position = new Vector3(0.0291f, 0.9633f, 0.7405f);  // Set arrow position to (0, 0, 0)
    Arrow.transform.rotation = Quaternion.Euler(-122.43f, -8.7f, 0); 
    Arrow.SetActive(true);  // Activate the arrow
    yield return new WaitForSeconds(delayBetweenSteps);
    yield return StartCoroutine(RemoveBattery(new Vector3(-0.5f, 0.05f, 0)));
    yield return new WaitForSeconds(delayBetweenSteps);
    Arrow.SetActive(false);  // Deactivate the arrow after the step


            UpdateInfoText("step: insert new battery");
            yield return StartCoroutine(InsertBattery(new Vector3(1, 0, 0)));

///////////////////////////////////
        UpdateInfoText("step: now zusammenbauen whatever 1");
yield return StartCoroutine(RotateCoverElectronicsBack(-180f));  // Rotate back by 90 degrees around the X-axis

            yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(-0.0251f, 1.0214f, 0.6565f), new Vector3(90, 0, 0)));

            UpdateInfoText("step: now zusammenbauen whatever 2");
         yield return StartCoroutine(ReturnScrews2(0.1f));  // Move screws back over 1 second

        yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(0.695f, 0.934f, 0.581f), new Vector3(0, 0, 0)));

        UpdateInfoText("step: now zusammenbauen whatever 3");
yield return StartCoroutine(ReturnCasingDiskSmooth(1f));  // Return CasingDisk over 1 second


            UpdateInfoText("step: now zusammenbauen whatever 4");
            yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(-0.0251f, 1.0214f, 0.6565f), new Vector3(90, 0, 0)));

         yield return StartCoroutine(ReturnScrews1(0.1f));  // Move screws back over 1 second

                yield return StartCoroutine(MoveScrewDriverSmooth(new Vector3(0.695f, 0.934f, 0.581f), new Vector3(0, 0, 0)));

            // yield return StartCoroutine(RemoveScrews1Reverse(new Vector3(-1, 0, 0)));


            UpdateInfoText("step: now zusammenbauen whatever 5");

           yield return StartCoroutine(RotateLaptopReverse(180f, Monitor.transform,customAxis, BatteryOld, BatteryNew));
        // yield return StartCoroutine(RotateLaptopReverse(180f, Monitor.transform, customAxis));
        yield return new WaitForSeconds(delayBetweenSteps);


       UpdateInfoText("fucking last Step: open laptop and test");
        yield return StartCoroutine(RotateMonitorReverse(90));  // Rotate by 90 degrees
        yield return new WaitForSeconds(delayBetweenSteps);
    }

    // Update the info text with the current step's description
    void UpdateInfoText(string message)
    {
        if (infoText3D != null)
        {
            infoText3D.text = message;
        }
    }

    IEnumerator RotateMonitorSmooth(float angle)
    {
        Quaternion startRotation = Monitor.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(-angle, 0, 0);
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            Monitor.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Monitor.transform.rotation = endRotation;
    }


IEnumerator RotateGroupSmooth(float angle, Transform referenceChild, Vector3 axis)
{
    Vector3 referencePoint = referenceChild.position;  // Der Punkt, um den rotiert wird
    float elapsedTime = 0;

    while (elapsedTime < rotationDuration)
    {
        // Berechne den Rotationswinkel pro Frame
        float step = (angle / rotationDuration) * Time.deltaTime;

        // Drehung um eine manuelle Achse
        Laptop.transform.RotateAround(referencePoint, axis, step);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Endrotation anwenden, um sicherzustellen, dass die Rotation genau abgeschlossen wird
    Laptop.transform.RotateAround(referencePoint, axis, angle - (angle * (elapsedTime / rotationDuration)));
}



Vector3 storedEdgePosition;


// Smooth rotation for CasingElectronics around the X-axis at a specific edge
IEnumerator RotateCoverElectronicsAroundEdge(float rotationAngle)
{
    // Get the bounds of the object to find the desired edge position
    Bounds bounds = CoverElectronics.GetComponent<Renderer>().bounds;
    
    // Calculate the edge position (either bounds.min or bounds.max, depending on the edge you want)
    Vector3 edgePosition = bounds.min; // or bounds.max for the opposite edge

    float elapsedTime = 0;

    // Perform the rotation over the specified duration
    while (elapsedTime < rotationDuration)
    {
        // Calculate the rotation step per frame
        float step = rotationAngle / rotationDuration * Time.deltaTime;

        // Rotate the object around the specified edge position using the X-axis
        CoverElectronics.transform.RotateAround(edgePosition, Vector3.right, step);  // Rotate around X-axis

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the object reaches the exact target rotation at the end
    // Rotate any remaining rotation (if the elapsed time didn't perfectly match rotationDuration)
    float remainingRotation = rotationAngle - (rotationAngle / rotationDuration * elapsedTime);
    CoverElectronics.transform.RotateAround(edgePosition, Vector3.right, remainingRotation);
}





    // Smooth movement for CasingDisk
    IEnumerator MoveCasingDiskSmooth(Vector3 movement)
    {
        initialCasingDiskPosition = CasingDisk.transform.position;

        Vector3 startPosition = CasingDisk.transform.position;
        Vector3 endPosition = movement;
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            CasingDisk.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        CasingDisk.transform.position = endPosition;
    }




IEnumerator MoveScrewDriverSmooth(Vector3 targetPosition, Vector3 targetRotationEuler)
{
    Vector3 startPosition = ScrewDriver.transform.position;  // Get the current position
    Quaternion startRotation = ScrewDriver.transform.rotation;  // Get the current rotation
    Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);  // Convert Euler angles to a quaternion

    float elapsedTime = 0;

    while (elapsedTime < rotationDuration)
    {
        // Smoothly interpolate the position towards the target position (absolute world position)
        ScrewDriver.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / rotationDuration);

        // Smoothly interpolate the rotation towards the target rotation (absolute rotation)
        ScrewDriver.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the final position and rotation are exactly at the target
    ScrewDriver.transform.position = targetPosition;
    ScrewDriver.transform.rotation = targetRotation;
}



    // Coroutine to remove screws and store their initial positions
    public IEnumerator RemoveScrews123(Vector3 worldTargetPosition, float screwMoveDuration = 1f)
    {
        // Loop through each child of Screws1
        foreach (Transform screw in Screws1.transform)
        {
            // Store the original position in the dictionary
            if (!originalPositions.ContainsKey(screw))
            {
                originalPositions[screw] = screw.position;  // Store initial position
            }

            Vector3 startPosition = screw.position;  // World position
            float elapsedTime = 0;

            while (elapsedTime < screwMoveDuration)
            {
                // Smoothly interpolate the world position of the screw
                screw.position = Vector3.Lerp(startPosition, worldTargetPosition, elapsedTime / screwMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the screw reaches the exact world target position
            screw.position = worldTargetPosition;

            // Log final world position to verify
            Debug.Log("Final Screw World Position: " + screw.position);
        }
    }



    // Coroutine to remove screws, move the screwdriver, and store the initial positions
    public IEnumerator RemoveScrews1(Vector3 worldTargetPosition, float screwMoveDuration = 1f)
    {
        // Loop through each child of Screws1
        foreach (Transform screw in Screws1.transform)
        {
            // Store the original position in the dictionary
            if (!originalPositions.ContainsKey(screw))
            {
                originalPositions[screw] = screw.position;  // Store initial position
            }

            Vector3 startPosition = screw.position;  // World position
            float elapsedTime = 0;

            // Move the screwdriver to the screw's position with an offset
            yield return StartCoroutine(MoveScrewdriver(screw.position + screwdriverOffset, screwMoveDuration * 0.5f));

            while (elapsedTime < screwMoveDuration)
            {
                // Smoothly interpolate the world position of the screw
                screw.position = Vector3.Lerp(startPosition, worldTargetPosition, elapsedTime / screwMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the screw reaches the exact world target position
            screw.position = worldTargetPosition;

            // Log final world position to verify
            Debug.Log("Final Screw World Position: " + screw.position);
        }
    }


    // Coroutine to move the screwdriver to a target position
    private IEnumerator MoveScrewdriver(Vector3 targetPosition, float moveDuration)
    {
        Vector3 startPosition = ScrewDriver.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            // Smoothly move the screwdriver to the target position
            ScrewDriver.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the screwdriver reaches the exact target position
        ScrewDriver.transform.position = targetPosition;
    }



    // Coroutine to remove screws, move the screwdriver, and store the initial positions
    public IEnumerator RemoveScrews2(Vector3 worldTargetPosition, float screwMoveDuration = 1f)
    {
        // Loop through each child of Screws1
        foreach (Transform screw in Screws2.transform)
        {
            // Store the original position in the dictionary
            if (!originalPositions.ContainsKey(screw))
            {
                originalPositions[screw] = screw.position;  // Store initial position
            }

            Vector3 startPosition = screw.position;  // World position
            float elapsedTime = 0;

            // Move the screwdriver to the screw's position with an offset
            yield return StartCoroutine(MoveScrewdriver(screw.position + screwdriverOffset, screwMoveDuration * 0.5f));

            while (elapsedTime < screwMoveDuration)
            {
                // Smoothly interpolate the world position of the screw
                screw.position = Vector3.Lerp(startPosition, worldTargetPosition, elapsedTime / screwMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the screw reaches the exact world target position
            screw.position = worldTargetPosition;

            // Log final world position to verify
            Debug.Log("Final Screw World Position: " + screw.position);
        }
    }



        IEnumerator UnplugCoverElectronics(Vector3 movement)
        {
            Vector3 startPos = CoverElectronics.transform.position;
            Vector3 endPos = startPos + movement;
            float elapsedTime = 0;

            while (elapsedTime < rotationDuration)
            {
                CoverElectronics.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            CoverElectronics.transform.position = endPos;

        }


        IEnumerator RemoveBattery(Vector3 movement)
        {
            Vector3 startPos = BatteryOld.transform.position;
            storedVector = startPos;
            Vector3 endPos = startPos + movement;
            float elapsedTime = 0;

            while (elapsedTime < rotationDuration)
            {
                BatteryOld.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            BatteryOld.transform.position = endPos;

        }


//


        IEnumerator InsertBattery(Vector3 movement)
        {
            Vector3 startPos = BatteryNew.transform.position;
            
            //Vector3 endPos = startPos + movement;
            Vector3 endPos = storedVector;
            float elapsedTime = 0;

            while (elapsedTime < rotationDuration)
            {
                BatteryNew.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            BatteryNew.transform.position = endPos;

        }

IEnumerator RemoveScrews2Reverse(Vector3 movement)
{
    Vector3 startPosition = Screws2.transform.position;
    Vector3 endPosition = startPosition - movement;  // Rückwärtsbewegung
    float elapsedTime = 0;

    while (elapsedTime < rotationDuration)
    {
        Screws2.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / rotationDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    Screws2.transform.position = endPosition;
}


// Smooth movement to return CasingDisk to its original position
IEnumerator ReturnCasingDiskSmooth(float moveDuration)
{
    // Ensure the original position is stored
    if (initialCasingDiskPosition == null)
    {
        Debug.LogWarning("Initial position of CasingDisk is not stored.");
        yield break;
    }

    Vector3 startPosition = CasingDisk.transform.position;
    Vector3 endPosition = initialCasingDiskPosition;
    float elapsedTime = 0;

    while (elapsedTime < moveDuration)
    {
        // Smoothly interpolate the position of CasingDisk back to its initial position
        CasingDisk.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the CasingDisk reaches the exact original position
    CasingDisk.transform.position = endPosition;
}




IEnumerator RemoveScrews1Reverse(float screwMoveDuration = 1f)
{
    // Loop through each child of Screws1
    foreach (Transform screw in Screws1.transform)
    {
        // Store the current position as the target position for moving back
        Vector3 targetPosition = screw.position;

        // Retrieve the initial position
        Vector3 originalPosition = screw.position; // Modify this part to access the originalPositions stored in RemoveScrews1

        float elapsedTime = 0;

        while (elapsedTime < screwMoveDuration)
        {
            // Smoothly interpolate back to the original position
            screw.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / screwMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the screw reaches the exact original position
        screw.position = originalPosition;

        // Log final original position to verify
        Debug.Log("Screw returned to Original World Position: " + screw.position);
    }
}

IEnumerator ReverseRotateCoverElectronicsAroundEdge(float rotationAngle)
{
    // Use the stored edge position to ensure the same pivot is used
    Vector3 edgePosition = storedEdgePosition;

    // Create an empty GameObject to act as the pivot
    GameObject pivot = new GameObject("TemporaryPivot");
    
    // Set the pivot's position to the stored edge
    pivot.transform.position = edgePosition;

    // Parent the object to the pivot, preserving the world position
    CoverElectronics.transform.SetParent(pivot.transform, true); // 'true' preserves the world position

    // Get the initial rotation of the pivot
    Quaternion startRotation = pivot.transform.rotation;

    // Reverse the rotation (rotate in the opposite direction around the same axis)
    Quaternion endRotation = startRotation * Quaternion.Euler(-rotationAngle, 0, 0);

    float elapsedTime = 0;

    // Perform the reverse rotation over the specified duration
    while (elapsedTime < rotationDuration)
    {
        // Smoothly interpolate the reverse rotation of the pivot
        pivot.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotationDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the pivot reaches the exact reversed end rotation
    pivot.transform.rotation = endRotation;

    // Unparent the object while preserving its world position and rotation
    CoverElectronics.transform.SetParent(null, true); // 'true' preserves the world position and rotation when unparenting

    // Destroy the temporary pivot GameObject
    Destroy(pivot);
}


 
IEnumerator RotateLaptopReverse(float angle, Transform referenceChild, Vector3 axis, GameObject childToReplace, GameObject replacementChild)
{
    Vector3 referencePoint = referenceChild.position;  // The point around which to rotate
    float elapsedTime = 0;

    // Ensure the replacement child is disabled initially
    if (replacementChild != null)
    {
        replacementChild.SetActive(false);
    }

    // Make the replacementChild a child of the Laptop if needed
    if (replacementChild != null)
    {
        replacementChild.transform.SetParent(Laptop.transform);  // Make replacementChild a child of Laptop
        childToReplace.transform.SetParent(null);  // Ensure the child is unparented
    }

    while (elapsedTime < rotationDuration)
    {
        // Calculate the rotation step per frame (reverse)
        float step = (-angle / rotationDuration) * Time.deltaTime;  // Negative angle for reverse rotation

        // Rotate the laptop around the reference point
        Laptop.transform.RotateAround(referencePoint, axis, step);

        elapsedTime += Time.deltaTime;
        yield return null;

        // At some point during the reverse rotation, unparent childToReplace and enable replacementChild
        if (elapsedTime > rotationDuration / 2 && childToReplace != null && replacementChild != null)
        {
            // Unparent the old child and enable the new one
            childToReplace.transform.SetParent(null);  // Unparent childToReplace from Laptop
            replacementChild.SetActive(true);
        }
    }

    // Ensure the final rotation is applied exactly (reverse final rotation)
    Laptop.transform.RotateAround(referencePoint, axis, -angle - (-angle * (elapsedTime / rotationDuration)));

}

 
    IEnumerator RotateMonitorReverse(float angle)
    {
        Quaternion startRotation = Monitor.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(-angle, 0, 0);
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            Monitor.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Monitor.transform.rotation = endRotation;
    }





// Coroutine to return screws to their original positions and move the screwdriver
public IEnumerator ReturnScrews1(float screwMoveDuration = 1f)
{
    // Loop through each child of Screws1
    foreach (Transform screw in Screws1.transform)
    {
        // Check if the original position is stored
        if (originalPositions.ContainsKey(screw))
        {
            Vector3 originalPosition = originalPositions[screw];  // Retrieve the initial position
            float elapsedTime = 0;

            // Move the screwdriver to the original position (before the screw returns)
            yield return StartCoroutine(MoveScrewdriver(originalPosition + screwdriverOffset, screwMoveDuration * 0.5f));

            while (elapsedTime < screwMoveDuration)
            {
                // Smoothly interpolate back to the original position
                screw.position = Vector3.Lerp(screw.position, originalPosition, elapsedTime / screwMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the screw reaches the exact original position
            screw.position = originalPosition;

            // Log final original position to verify
            Debug.Log("Screw returned to Original World Position: " + screw.position);
        }
        else
        {
            Debug.LogWarning("Original position not found for screw: " + screw.name);
        }
    }
}



// Coroutine to return screws to their original positions and move the screwdriver
public IEnumerator ReturnScrews2(float screwMoveDuration = 1f)
{
    // Loop through each child of Screws1
    foreach (Transform screw in Screws2.transform)
    {
        // Check if the original position is stored
        if (originalPositions.ContainsKey(screw))
        {
            Vector3 originalPosition = originalPositions[screw];  // Retrieve the initial position
            float elapsedTime = 0;

            // Move the screwdriver to the original position (before the screw returns)
            yield return StartCoroutine(MoveScrewdriver(originalPosition + screwdriverOffset, screwMoveDuration * 0.5f));

            while (elapsedTime < screwMoveDuration)
            {
                // Smoothly interpolate back to the original position
                screw.position = Vector3.Lerp(screw.position, originalPosition, elapsedTime / screwMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the screw reaches the exact original position
            screw.position = originalPosition;

            // Log final original position to verify
            Debug.Log("Screw returned to Original World Position: " + screw.position);
        }
        else
        {
            Debug.LogWarning("Original position not found for screw: " + screw.name);
        }
    }
}
    


    // Smooth reverse rotation for CasingElectronics around the X-axis at a specific edge
IEnumerator RotateCoverElectronicsBack(float rotationAngle)
{
    // Get the bounds of the object to find the desired edge position
    Bounds bounds = CoverElectronics.GetComponent<Renderer>().bounds;
    
    // Calculate the edge position (same edge as used in the forward rotation)
    Vector3 edgePosition = bounds.max; // or bounds.max, depending on the edge used for the forward rotation

    float elapsedTime = 0;

    // Perform the reverse rotation over the specified duration
    while (elapsedTime < rotationDuration)
    {
        // Calculate the rotation step per frame (reverse)
        float step = (-rotationAngle / rotationDuration) * Time.deltaTime;  // Negative step for reverse rotation

        // Rotate the object around the specified edge position using the X-axis in reverse
        CoverElectronics.transform.RotateAround(edgePosition, Vector3.right, step);  // Rotate around X-axis

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the object reaches the exact target rotation at the end
    // Rotate any remaining rotation (if the elapsed time didn't perfectly match rotationDuration)
    float remainingRotation = -rotationAngle - (-rotationAngle / rotationDuration * elapsedTime);
    CoverElectronics.transform.RotateAround(edgePosition, Vector3.right, remainingRotation);
}



}
