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
    public static void Throws(Action throwAction) {
        try {
            throwAction();
        } catch {
            return;
        }
        throw new Exception("Expected an exception to be thrown.");
    }
    public static void DoesNotThrow(Action nonThrowAction) {
        try {
            nonThrowAction();
        } catch (Exception inner) {
            throw new Exception("Expected no exception, but one was thrown.", inner);
        }
    }
}
