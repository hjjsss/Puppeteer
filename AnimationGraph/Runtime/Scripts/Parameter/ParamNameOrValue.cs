﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Parameter
{
    /// <summary>
    /// Only used for serialization.
    /// </summary>
    [Serializable]
    public class ParamNameOrValue
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [SerializeField]
        private string _name;


        public float RawValue
        {
            get => _rawValue;
            set => _rawValue = value;
        }

        [SerializeField]
        private float _rawValue = 1f;


        public bool IsValue => string.IsNullOrEmpty(Name);


        public ParamNameOrValue(string name, float rawValue)
        {
            _name = name;
            _rawValue = rawValue;
        }

        public ParamNameOrValue(ParamInfo paramInfo) : this(paramInfo.Name, paramInfo.RawValue)
        {
        }


        public float GetFloat()
        {
            Assert.IsTrue(IsValue);

            return _rawValue;
        }

        public int GetInt()
        {
            Assert.IsTrue(IsValue);

            return (int)Math.Round(_rawValue);
        }

        public bool GetBool()
        {
            Assert.IsTrue(IsValue);

            return Mathf.Approximately(_rawValue, 1);
        }


        public ParamInfo GetParamInfo(IDictionary<string, ParamInfo> paramTable, ParamType paramType)
        {
            if (IsValue)
            {
                return new ParamInfo(null, null, paramType, RawValue);
            }

            var paramInfo = paramTable[Name];
            Assert.IsTrue(paramInfo.Type == paramType);

            return paramInfo;
        }

        public ParamInfo GetParamInfo(IList<ParamInfo> paramTable, ParamType paramType)
        {
            if (IsValue)
            {
                return new ParamInfo(null, null, paramType, RawValue);
            }

            for (int i = 0; i < paramTable.Count; i++)
            {
                if (paramTable[i].Name.Equals(Name))
                {
                    var paramInfo = paramTable[i];
                    Assert.IsTrue(paramInfo.Type == paramType);
                    return paramInfo;
                }
            }

            return null;
        }
    }
}
