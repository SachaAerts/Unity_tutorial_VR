using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace LabXR.Scripts.UI
{
    public class Annotation : MonoBehaviour
    {
        [SerializeField] private Transform followTransform;
        [SerializeField] private Transform objectTransform;
        [SerializeField] private string defaultName = "Annotation";

        [Header("Prefab Settings")]
        [SerializeField] private float baseScale = 0.002f;

        [Space]
        [SerializeField] private TextMeshProUGUI[] texts;

        [SerializeField] private GameObject leftWidget;
        [SerializeField] private GameObject rightWidget;
        private Camera _camera;

        private float followPointRadius;

        private void Start()
        {
            _camera = Camera.main;
            foreach (var text in texts)
            {
                text.text = defaultName;
            }
            followPointRadius = Vector3.Distance(objectTransform.position, followTransform.position);
        }

        void SetupAnnotation(Transform _followTransform, string _name)
        {
            followTransform = _followTransform;
            foreach (var text in texts)
            {
                text.text = _name;
            }
        }

        void Update()
        {
            if (!_camera) return;

            transform.position = followTransform.position;
            transform.LookAt(_camera.transform);
            transform.Rotate(0, 180f, 0);
            float currentScale = Vector3.Distance(transform.position, _camera.transform.position) * baseScale;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            
            bool isLeft = _camera.WorldToScreenPoint(objectTransform.position).x>_camera.WorldToScreenPoint(followTransform.position).x;
            leftWidget.SetActive(isLeft);
            rightWidget.SetActive(!isLeft);

            float objectDistance = Vector3.Distance(objectTransform.position, _camera.transform.position);
            float followPointDistance = Vector3.Distance(followTransform.position, _camera.transform.position);

            float Depth = Mathf.Clamp(objectDistance - followPointDistance, -1, 1);
            float Opacity = math.remap(-1,1,0,1,Depth);
            
            
            //Set Opacity on text material 


        }
    }
}