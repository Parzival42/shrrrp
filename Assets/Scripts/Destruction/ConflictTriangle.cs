using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConflictTriangle {

	public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public bool aLeft;
        public bool bLeft;
        public bool cLeft;

        public bool negative;

        public ConflictTriangle(Vector3 a, Vector3 b, Vector3 c, bool aLeft, bool bLeft, bool cLeft, bool negative){
            this.a = a;
            this.b = b;
            this.c = c;

            this.aLeft = aLeft;
            this.bLeft = bLeft;
            this.cLeft = cLeft;

            this.negative = negative;
        }
}
