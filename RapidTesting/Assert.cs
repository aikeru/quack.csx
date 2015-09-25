public class Assert {
    public static void IsTrue(bool exp) {
        if(!exp) { throw new Exception("Expected true but was false."); }
    }
    public static void IsFalse(bool exp) {
    	if(exp) { throw new Exception("Expected false but was true."); }
    }
    public static void IsNull(object obj) {
    	if(obj != null) { throw new Exception("Expected null, but was non-null."); }
    }
    public static void IsNotNull(object obj) {
        if(obj == null) { throw new Exception("Expected non-null, but was null."); }
    }
}
