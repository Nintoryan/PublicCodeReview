using System;
using SO.Data;
using UnityEngine;
using VillageGame.UI;
using VillageGame.UI.Controllers;
using Web.ResponseStructs.PayloadValues;

namespace VillageGame.Logic.Buildings
{
    public class Building : MonoBehaviour, IEquatable<Building>
    {
        [SerializeField] private BuildingCanvasController _canvasController;
        [SerializeField] private GameObject _view;
        public BuildingLookPosition LookPosition;
        public Mainer Mainer{ get; private set; }
        public BuildingData Data { get; private set; }
        public BuildData BuildData{ get; private set; }
        public BuildingCanvasController CanvasController => _canvasController;

        private Coroutine _mainerCoroutine;

        private void OnDestroy()
        {
            Mainer?.StopUpdateMainer();
            StopUpdateMainer();
        }

        public void Initial(BuildingData data,InputBlockService inputBlockService)
        {
            Data = data;
            _canvasController?.SetBlocker(inputBlockService);
            Mainer = new Mainer(data.MainerData);
        }

        public void SetBuildData(BuildData data)
        {
            BuildData = data;
        }

        public void StartUpdateMainer()
        {
            _mainerCoroutine = StartCoroutine(Mainer.StartUpdateMainer());
        }

        private void StopUpdateMainer()
        {
            if (_mainerCoroutine != null)
            {
                StopCoroutine(_mainerCoroutine);
            }
        }

        public void ActiveSprite(bool isActive)
        {
            _view.gameObject.SetActive(isActive);
        }
        public void DestroyYourself()
        {
            StopUpdateMainer();
            Destroy(gameObject);
        }
        
        public bool Equals(Building other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(Data, other.Data) && BuildData.Equals(other.BuildData);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Building)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Data, BuildData);
        }
    }
}