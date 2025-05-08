using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;  // For AR Image Tracking
using UnityEngine.XR.ARSubsystems;  // For image tracking types
using TMPro;


// todo scheiss auf den arrow

public class StepManagerAR : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject laptopPrefab;
    public GameObject screwdriverPrefab;  // Add screwdriver prefab reference
        public GameObject newBatteryPrefab;  // Add  prefab reference

    private GameObject spawnedLaptop;        
    private Vector3 storedVector = new Vector3(0, 0, 0);  // Klassenvariable

    private GameObject spawnedScrewDriver;  // Track the spawned screwdriver
    private GameObject spawnedBatteryNew;
    private bool laptopSpawned = false;
    private Vector3 screwdriverOffset = new Vector3(0, 0.118f, 0);  


    private Vector3 initialCasingDiskPosition = new Vector3(0,0,0);
    public GameObject ScrewDriver;
    public GameObject BatteryNew;
    public GameObject Arrow;
    public GameObject Monitor;
    public GameObject Screws1;
    public GameObject Screws2;
    public GameObject BatteryOld;
    public GameObject CoverElectronics;
    public GameObject CasingDisk;

    public TextMeshProUGUI infoText;

    public float delayBetweenSteps = 1f;
    public float rotationDuration = 1f;

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            HandleTrackedImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            HandleTrackedImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            if (spawnedLaptop != null)
            {
                Destroy(spawnedLaptop);
                laptopSpawned = false;
            }

            if (spawnedScrewDriver != null)
            {
                Destroy(spawnedScrewDriver);
            }

            if (spawnedBatteryNew != null)
            {
                Destroy(spawnedBatteryNew);
            }
        }
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking && !laptopSpawned)
        {
            // Ensure laptop is only spawned once
            SpawnLaptop(trackedImage.transform.position, trackedImage.transform.rotation);
        }
    }


void SpawnLaptop(Vector3 position, Quaternion rotation)
{
    if (!laptopSpawned)
    {
        // Adjust the position and rotation relative to the tracked image or environment
        Vector3 adjustedPosition = position + new Vector3(-2.31f, -1.8066f, 0.66f); // Adjust the Y-axis to lift the laptop slightly above the plane/image
        Quaternion adjustedRotation = rotation * Quaternion.Euler(180, 90, 0); // Adjust the rotation to face the right direction
        Quaternion adjustedRotationBattery = rotation * Quaternion.Euler(270, 180, -90); // Adjust the rotation to face the right direction
// 0.9200005   -0.845
// 2.2336  0.427
// 0.813   0.031
        // Instantiate the laptop prefab with the adjusted position and rotation
        spawnedLaptop = Instantiate(laptopPrefab, adjustedPosition, adjustedRotation);
        AssignChildReferences();  // Dynamically assign child references from the laptop

        // Spawn the screwdriver near the laptop with similar ad justments
        Vector3 screwdriverPosition = new Vector3(-2.25f, -0.355f, 0.319f); // Adjust this position as needed // adjustedPosition + 
        Quaternion screwdriverRotation = adjustedRotation; // Ensure the screwdriver has the same orientation
        spawnedScrewDriver = Instantiate(screwdriverPrefab, screwdriverPosition, screwdriverRotation);

        // Instantiate the battery and store its reference
        Vector3 newBatteryPosition = new Vector3(-2.53f, -0.355f, 0.154f); 
        Quaternion newBatteryRotation = adjustedRotationBattery; // Set appropriate rotation
        spawnedBatteryNew = Instantiate(newBatteryPrefab, newBatteryPosition, newBatteryRotation);

        // Start the steps sequence only after the laptop, screwdriver, and battery are spawned
        StartCoroutine(PerformStepsSequence());

        // Set the flag to true to prevent multiple spawns
        laptopSpawned = true;

        Debug.Log("Laptop, Screwdriver, and BatteryNew successfully spawned with adjusted position and rotation.");
    }
    else
    {
        Debug.LogWarning("Laptop is already spawned. Skipping.");
    }
}



    // Dynamically assign all child objects from the laptop prefab
    void AssignChildReferences()
    {
        if (spawnedLaptop != null)
        {
            Monitor = spawnedLaptop.transform.Find("Monitor")?.gameObject;
            CasingDisk = spawnedLaptop.transform.Find("CasingDisk")?.gameObject;
            Screws1 = spawnedLaptop.transform.Find("Screws1")?.gameObject;
            Screws2 = spawnedLaptop.transform.Find("Screws2")?.gameObject;
            BatteryOld = spawnedLaptop.transform.Find("BatteryOld")?.gameObject;
            CoverElectronics = spawnedLaptop.transform.Find("CoverElectronics")?.gameObject;

            if (Monitor == null || CasingDisk == null || Screws1 == null || Screws2 == null || BatteryOld == null || CoverElectronics == null)
            {
                Debug.LogError("One or more required components are missing from the prefab.");
            }
        }

        if (Arrow != null)
        {
            Arrow.SetActive(false);  // Initially hide the arrow
        }
    }

    void UpdateInfoText(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;  // Update the UI Text
        }
        else
        {
            Debug.LogWarning("InfoText reference is missing!");
        }
    }

    // Coroutine to perform each step in sequence
   
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
        yield return StartCoroutine(MoveScrewDriver(new Vector3(-2.771f, -0.22f, 0.74f), new Vector3(90, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);


        // Step : Move CasingElectronics to the left
        UpdateInfoText("Step 4: Remove all visible Screws");
        Vector3 targetPositionScrews1 = new Vector3(-2.5f, -0.55f, 0.319f);  // Replace with the actual target position
        yield return StartCoroutine(RemoveScrews1(targetPositionScrews1, 0.1f));  // Move screws over 1 second
            yield return new WaitForSeconds(delayBetweenSteps);

        // yield return StartCoroutine(RemoveScrews1(new Vector3(-0.325f, 0.887f, 0.654f))); // xxx todo
        yield return new WaitForSeconds(delayBetweenSteps);
        yield return StartCoroutine(MoveScrewDriver(new Vector3(-2.25f, -0.355f, 0.319f), new Vector3(-90, 0, 0)));


        // Step : Move CasingDisk to the left and down
        UpdateInfoText("Step 4: remove CasingDisk");
                Vector3 targetPositionCasingDisk = new Vector3(-2.5f, 0f, 1.282f);  // Replace with the actual target position

        yield return StartCoroutine(MoveCasingDiskSmooth(targetPositionCasingDisk));


        // Step : Move CasingElectronics to the left
        UpdateInfoText("Step 3: Take the screwdriver");
         yield return StartCoroutine(MoveScrewDriver(new Vector3(-2.771f, -0.22f, 1.038f), new Vector3(90, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);


                // Step : Move CasingElectronics to the left
        UpdateInfoText("Step 4: RemoveScrews2");


        Vector3 targetPositionScrews2 = new Vector3(-2.5f, -0.55f, 0.319f);  // Replace with the actual target position
        yield return StartCoroutine(RemoveScrews2(targetPositionScrews2, 0.1f));  // Move screws over 1 second



        // yield return StartCoroutine(RemoveScrews2(new Vector3(-1, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);
                yield return StartCoroutine(MoveScrewDriver(new Vector3(-2.25f, -0.355f, 0.319f), new Vector3(-90, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);


    // Step 5: Move CasingDisk and Show Arrow at (0.184, 0.792, 0.555)
    UpdateInfoText("Step 5: unplug cover electronics, start at green arrow");
    Arrow.transform.position = new Vector3(0.184f, 0.792f, 0.555f);  // Set arrow position
    Arrow.SetActive(true);  // Activate the arrow
        yield return new WaitForSeconds(delayBetweenSteps);
// Assuming the rectangular object has a length of 5 units and you want to rotate around one of the longer edges
Vector3 pivotOffset = new Vector3(0, 0, -0.4585f); // Adjust this to the correct edge
yield return StartCoroutine(RotateCoverElectronicsAroundEdge(180f));  // Rotates 90 degrees around the calculated edge


        // yield return StartCoroutine(RotateCoverElectronics(-180f));
    yield return new WaitForSeconds(delayBetweenSteps);
    Arrow.SetActive(false);  // Deactivate the arrow after the step

    // Step 6: Unplug cover electronics and Show Arrow at (0, 0, 0)
    UpdateInfoText("Step 6: remove cable at green arrow, remove battery");
    Arrow.transform.position = new Vector3(0.0291f, 0.9633f, 0.7405f);  // Set arrow position to (0, 0, 0)
    Arrow.transform.rotation = Quaternion.Euler(-122.43f, -8.7f, 0); 
    Arrow.SetActive(true);  // Activate the arrow
    yield return new WaitForSeconds(delayBetweenSteps);
    yield return StartCoroutine(RemoveBattery(new Vector3(0.427f, 0.05f, 0)));
    yield return new WaitForSeconds(delayBetweenSteps);
    Arrow.SetActive(false);  // Deactivate the arrow after the step

        yield return new WaitForSeconds(delayBetweenSteps);

            UpdateInfoText("step: insert new battery");
            yield return StartCoroutine(InsertBattery(new Vector3(1, 1, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);

///////////////////////////////////
        UpdateInfoText("step: now zusammenbauen whatever 1");
yield return StartCoroutine(RotateCoverElectronicsBack(180f));  // Rotate back by 90 degrees around the X-axis

        yield return StartCoroutine(MoveScrewDriver(new Vector3(-2.771f, -0.22f, 0.74f), new Vector3(90, 0, 0)));

            UpdateInfoText("step: now zusammenbauen whatever 2");
         yield return StartCoroutine(ReturnScrews2(0.1f));  // Move screws back over 1 second





        yield return StartCoroutine(MoveScrewDriver(new Vector3(-2.25f, -0.355f, 0.319f), new Vector3(-90, 0, 0)));

        UpdateInfoText("step: now zusammenbauen whatever 3");
yield return StartCoroutine(ReturnCasingDiskSmooth(1f));  // Return CasingDisk over 1 second


            UpdateInfoText("step: now zusammenbauen whatever 4");
        yield return StartCoroutine(MoveScrewDriver(new Vector3(-2.771f, -0.22f, 0.74f), new Vector3(90, 0, 0)));

         yield return StartCoroutine(ReturnScrews1(0.1f));  // Move screws back over 1 second

                yield return StartCoroutine(MoveScrewDriver(new Vector3(0.695f, 0.934f, 0.581f), new Vector3(0, 0, 0)));

            // yield return StartCoroutine(RemoveScrews1Reverse(new Vector3(-1, 0, 0)));
        yield return new WaitForSeconds(delayBetweenSteps);


            UpdateInfoText("step: now zusammenbauen whatever 5");

           yield return StartCoroutine(RotateLaptopReverse(180f, Monitor.transform,customAxis, BatteryOld, BatteryNew));
        // yield return StartCoroutine(RotateLaptopReverse(180f, Monitor.transform, customAxis));
        yield return new WaitForSeconds(delayBetweenSteps);


       UpdateInfoText("fucking last Step: open laptop and test");
        yield return StartCoroutine(RotateMonitorReverse(90));  // Rotate by 90 degrees
        yield return new WaitForSeconds(delayBetweenSteps);




    }

    // Smoothly rotate the monitor
    IEnumerator RotateMonitorSmooth(float angle)
    {
        if (Monitor == null)
        {
            Debug.LogError("Monitor is not assigned.");
            yield break;
        }

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

    // Smoothly rotate the laptop group
    IEnumerator RotateGroupSmooth(float angle, Transform referenceChild, Vector3 axis)
    {
        Vector3 referencePoint = referenceChild.position;
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            float step = (angle / rotationDuration) * Time.deltaTime;
            spawnedLaptop.transform.RotateAround(referencePoint, axis, step);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spawnedLaptop.transform.RotateAround(referencePoint, axis, angle - (angle * (elapsedTime / rotationDuration)));
    }

    // Smoothly move the screwdriver (Screwdriver is not part of the laptop's children)
    IEnumerator MoveScrewDriver(Vector3 targetPosition, Vector3 targetRotationEuler)
    {
        if (spawnedScrewDriver == null)
        {
            Debug.LogWarning("Screwdriver not assigned.");
            yield break;
        }

        Vector3 startPosition = spawnedScrewDriver.transform.position;
        Quaternion startRotation = spawnedScrewDriver.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            spawnedScrewDriver.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / rotationDuration);
            spawnedScrewDriver.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spawnedScrewDriver.transform.position = targetPosition;
        spawnedScrewDriver.transform.rotation = targetRotation;
    }


// // Smooth rotation for CasingElectronics around the X-axis at a specific edge
// IEnumerator RotateCoverElectronicsAroundEdge(float rotationAngle)
// {
//     // Get the bounds of the object to find the desired edge position
//     Bounds bounds = CoverElectronics.GetComponent<Renderer>().bounds;
    
//     // Calculate the edge position (either bounds.min or bounds.max, depending on the edge you want)
//     Vector3 edgePosition = bounds.min; // or bounds.max for the opposite edge

//     float elapsedTime = 0;

//     // Perform the rotation over the specified duration
//     while (elapsedTime < rotationDuration)
//     {
//         // Calculate the rotation step per frame
//         float step = rotationAngle / rotationDuration * Time.deltaTime;

//         // Rotate the object around the specified edge position using the X-axis
//         CoverElectronics.transform.RotateAround(edgePosition, Vector3.forward, step);  // Rotate around X-axis

//         elapsedTime += Time.deltaTime;
//         yield return null;
//     }

//     // Ensure the object reaches the exact target rotation at the end
//     // Rotate any remaining rotation (if the elapsed time didn't perfectly match rotationDuration)
//     float remainingRotation = rotationAngle - (rotationAngle / rotationDuration * elapsedTime);
//     CoverElectronics.transform.RotateAround(edgePosition, Vector3.right, remainingRotation);
// }


// IEnumerator RotateCoverElectronicsAroundEdge(float rotationAngle)
// {
//     // Get the initial rotation of the object
//     Quaternion initialRotation = CoverElectronics.transform.rotation;

//     float elapsedTime = 0;

//     // Perform the rotation over the specified duration
//     while (elapsedTime < rotationDuration)
//     {
//         // Calculate the rotation step per frame
//         float step = rotationAngle / rotationDuration * Time.deltaTime;

//         // Rotate the object around its local X-axis without affecting position
//         CoverElectronics.transform.Rotate(Vector3.right, step);  // Rotate around X-axis (local axis)

//         elapsedTime += Time.deltaTime;
//         yield return null;
//     }

//     // Ensure the object reaches the exact target rotation at the end
//     // Rotate any remaining rotation (if the elapsed time didn't perfectly match rotationDuration)
//     float remainingRotation = rotationAngle - (rotationAngle / rotationDuration * elapsedTime);
//     CoverElectronics.transform.Rotate(Vector3.right, remainingRotation);
// }
IEnumerator RotateCoverElectronicsAroundEdge(float rotationAngle)
{
    // Get the bounds of the object to find the desired edge position
    Bounds bounds = CoverElectronics.GetComponent<Renderer>().bounds;

    // Calculate the correct edge position (for example, using bounds.min for one edge or bounds.max for the opposite)
    // In this case, assuming you want to rotate around the bottom-right long edge
    Vector3 edgePosition = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z); // Adjust this based on your specific edge

    float elapsedTime = 0;

    // Perform the rotation over the specified duration
    while (elapsedTime < rotationDuration)
    {
        // Calculate the rotation step per frame
        float step = rotationAngle / rotationDuration * Time.deltaTime;

        // Rotate the object around the specified edge position using the X-axis (for example, rotating around the long side)
        CoverElectronics.transform.RotateAround(edgePosition, Vector3.forward, step);  // Rotate around the X-axis

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the object reaches the exact target rotation at the end
    float remainingRotation = rotationAngle - (rotationAngle / rotationDuration * elapsedTime);
    CoverElectronics.transform.RotateAround(edgePosition, Vector3.forward, remainingRotation);
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
    // Coroutine to remove screws, move the screwdriver, and store the initial positions


public IEnumerator RemoveScrews1(Vector3 worldTargetPosition, float screwMoveDuration = .01f)
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

        Vector3 screwdriverTargetPosition = screw.position + screwdriverOffset;  // Adjust this offset as needed
        Vector3 screwdriverTargetRotation = new Vector3(90, 0, 0);  // Adjust this based on your desired screwdriver rotation

        yield return StartCoroutine(MoveScrewDriverFast(screwdriverTargetPosition, screwdriverTargetRotation));

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
public IEnumerator RemoveScrews2(Vector3 worldTargetPosition, float screwMoveDuration = 2f)
{
    // Loop through each child of Screws2
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
        Vector3 screwdriverTargetPosition = screw.position + screwdriverOffset;  // Adjust this offset as needed
        Vector3 screwdriverTargetRotation = new Vector3(90, 0, 0);  // Adjust this based on your desired screwdriver rotation

        yield return StartCoroutine(MoveScrewDriverFast(screwdriverTargetPosition, screwdriverTargetRotation));

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


// IEnumerator RemoveBattery(Vector3 movement)
// {
//     if (BatteryOld == null)
//     {
//         Debug.LogError("BatteryOld is not assigned.");
//         yield break;
//     }

//     // Get the initial position of BatteryOld and store it for later use
//     Vector3 startPos = BatteryOld.transform.position;
//     storedVector = startPos;  // Store the position to move the new battery here later

//     // Calculate the end position based on the movement vector
//     Vector3 endPos = startPos + movement;
//     float elapsedTime = 0;

//     // Log initial and target positions for debugging
//     Debug.Log("Removing BatteryOld - Start Position: " + startPos);
//     Debug.Log("Removing BatteryOld - Target Position: " + endPos);

//     // Move BatteryOld to the new position over the specified time
//     while (elapsedTime < rotationDuration)
//     {
//         BatteryOld.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / rotationDuration);
//         elapsedTime += Time.deltaTime;
//         yield return null;
//     }

//     // Ensure final position is set correctly
//     BatteryOld.transform.position = endPos;
//     Debug.Log("BatteryOld Removed - Final Position: " + BatteryOld.transform.position);
// }


// //
// IEnumerator InsertBattery(Vector3 movement)
// {
//     if (BatteryNew == null)
//     {
//         Debug.LogError("BatteryNew is not assigned.");
//         yield break;
//     }

//     // Get the initial position of BatteryNew
//     Vector3 startPos = BatteryNew.transform.position;

//     // Move the new battery to the stored position where the old battery was
//     Vector3 endPos = storedVector;  // Using the storedVector from RemoveBattery
//     float elapsedTime = 0;

//     // Log initial and target positions for debugging
//     Debug.Log("Inserting BatteryNew - Start Position: " + startPos);
//     Debug.Log("Inserting BatteryNew - Target Position: " + endPos);

//     // Move BatteryNew to the stored position
//     while (elapsedTime < rotationDuration)
//     {
//         BatteryNew.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / rotationDuration);
//         elapsedTime += Time.deltaTime;
//         yield return null;
//     }

//     // Ensure final position is set correctly
//     BatteryNew.transform.position = endPos;
//     Debug.Log("BatteryNew Inserted - Final Position: " + BatteryNew.transform.position);
// }

IEnumerator RemoveBattery(Vector3 movement)
{
    if (BatteryOld == null)
    {
        Debug.LogError("BatteryOld is not assigned.");
        yield break;
    }

    // Get the initial position of BatteryOld and store it for later use
    Vector3 startPos = BatteryOld.transform.position;
    storedVector = startPos;  // Store the position to move the new battery here later

    // Calculate the end position based on the movement vector
    Vector3 endPos = startPos + movement;
    float elapsedTime = 0;

    // Log initial and target positions for debugging
    Debug.Log("Removing BatteryOld - Start Position: " + startPos);
    Debug.Log("Removing BatteryOld - Target Position: " + endPos);

    // Move BatteryOld to the new position over the specified time
    while (elapsedTime < rotationDuration)
    {
        BatteryOld.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / rotationDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure final position is set correctly
    BatteryOld.transform.position = endPos;
    Debug.Log("BatteryOld Removed - Final Position: " + BatteryOld.transform.position);
}

IEnumerator InsertBattery(Vector3 movement)
{
    if (spawnedBatteryNew == null)
    {
        Debug.LogError("BatteryNew is not assigned.");
        yield break;
    }

    // Start unparented, so the battery doesn't move with the laptop
    spawnedBatteryNew.transform.SetParent(null);

    // Get the initial position of BatteryNew
    Vector3 startPos = spawnedBatteryNew.transform.position;

    // Move the new battery to the stored position where the old battery was
    Vector3 endPos = storedVector;  // Using the storedVector from RemoveBattery
    float elapsedTime = 0;

    Debug.Log($"Inserting BatteryNew - Start Position: {startPos}, Target Position: {endPos}");

    // Move BatteryNew to the stored position
    while (elapsedTime < rotationDuration)
    {
        spawnedBatteryNew.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / rotationDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure final position is set correctly
    spawnedBatteryNew.transform.position = endPos;
    Debug.Log($"BatteryNew Inserted - Final Position: {spawnedBatteryNew.transform.position}");

    // Now parent the battery to the laptop so it moves with the laptop from this point on
    spawnedBatteryNew.transform.SetParent(spawnedLaptop.transform);

    Debug.Log("BatteryNew is now parented to the laptop.");
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




IEnumerator RemoveScrews1Reverse(float screwMoveDuration = .1f)
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
       // Debug.Log("Screw returned to Original World Position: " + screw.position);
    }
}


IEnumerator ReverseRotateCoverElectronicsAroundEdge(float rotationAngle)
{
    if (CoverElectronics == null)
    {
        Debug.LogError("CoverElectronics is not assigned or instantiated.");
        yield break;
    }

    // Use the stored edge position to ensure the same pivot is used
    Vector3 storedEdgePosition = CoverElectronics.GetComponent<Renderer>().bounds.min; // or bounds.max depending on which edge you want
    Vector3 edgePosition = storedEdgePosition;

    // Create an empty GameObject to act as the pivot
    GameObject pivot = new GameObject("TemporaryPivot");
    
    // Set the pivot's position to the stored edge
    pivot.transform.position = edgePosition;

    // Make sure the CoverElectronics is not part of the prefab but the instance
    CoverElectronics = Instantiate(CoverElectronics, CoverElectronics.transform.position, CoverElectronics.transform.rotation);
    
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
        replacementChild.transform.SetParent(spawnedLaptop.transform);  // Make replacementChild a child of Laptop
        childToReplace.transform.SetParent(null);  // Ensure the child is unparented
    }

    while (elapsedTime < rotationDuration)
    {
        // Calculate the rotation step per frame (reverse)
        float step = (-angle / rotationDuration) * Time.deltaTime;  // Negative angle for reverse rotation

        // Rotate the laptop around the reference point
        spawnedLaptop.transform.RotateAround(referencePoint, axis, step);

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
    spawnedLaptop.transform.RotateAround(referencePoint, axis, -angle - (-angle * (elapsedTime / rotationDuration)));

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




public IEnumerator ReturnScrews1(float screwMoveDuration = .1f)
{
    // Loop through each child of Screws1
    foreach (Transform screw in Screws1.transform)
    {
        // Check if the original position is stored
        if (originalPositions.ContainsKey(screw))
        {
            Vector3 originalPosition = originalPositions[screw];  // Retrieve the initial position
            float elapsedTime = 0;

            // Move the screwdriver to the screw's position with an offset
            Vector3 screwdriverTargetPosition = originalPosition + screwdriverOffset;  // Adjust this offset as needed
            Vector3 screwdriverTargetRotation = new Vector3(90, 0, 0);  // Adjust based on your desired rotation for the screwdriver

            yield return StartCoroutine(MoveScrewDriverFast(screwdriverTargetPosition, screwdriverTargetRotation));
            // yield return StartCoroutine(MoveScrewdriver(screw.position + screwdriverOffset, screwMoveDuration * 0.5f));

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
          //  Debug.Log("Screw returned to Original World Position: " + screw.position);
        }
        else
        {
            Debug.LogWarning("Original position not found for screw: " + screw.name);
        }
    }
}


// Coroutine to return screws to their original positions and move the screwdriver
public IEnumerator ReturnScrews2(float screwMoveDuration = .1f)
{
    // Loop through each child of Screws2
    foreach (Transform screw in Screws2.transform)
    {
        // Check if the original position is stored
        if (originalPositions.ContainsKey(screw))
        {
            Vector3 originalPosition = originalPositions[screw];  // Retrieve the initial position
            float elapsedTime = 0;

            // Move the screwdriver to the original position (before the screw returns)
            Vector3 screwdriverTargetPosition = originalPosition + screwdriverOffset;  // Position of the screwdriver near the screw
            Vector3 screwdriverTargetRotation = new Vector3(90, 0, 0);  // Adjust based on your desired rotation for the screwdriver

            yield return StartCoroutine(MoveScrewDriverFast(screwdriverTargetPosition, screwdriverTargetRotation));

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
        //    Debug.Log("Screw returned to Original World Position: " + screw.position);
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
        CoverElectronics.transform.RotateAround(edgePosition, Vector3.forward, step);  // Rotate around X-axis

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Ensure the object reaches the exact target rotation at the end
    // Rotate any remaining rotation (if the elapsed time didn't perfectly match rotationDuration)
    float remainingRotation = -rotationAngle - (-rotationAngle / rotationDuration * elapsedTime);
    CoverElectronics.transform.RotateAround(edgePosition, Vector3.right, remainingRotation);
}


    // Smoothly move the screwdriver (Screwdriver is not part of the laptop's children)
    IEnumerator MoveScrewDriverFast(Vector3 targetPosition, Vector3 targetRotationEuler)
    {
        if (spawnedScrewDriver == null)
        {
            Debug.LogWarning("Screwdriver not assigned.");
            yield break;
        }

        Vector3 startPosition = spawnedScrewDriver.transform.position;
        Quaternion startRotation = spawnedScrewDriver.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration * 0.1f)
        {
            spawnedScrewDriver.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / (rotationDuration * 0.1f));
            spawnedScrewDriver.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / (rotationDuration * 0.1f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spawnedScrewDriver.transform.position = targetPosition;
        spawnedScrewDriver.transform.rotation = targetRotation;
    }




}
