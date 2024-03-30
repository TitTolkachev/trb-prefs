using FirebaseAdmin.Auth;

namespace trb_prefs.Common;

public static class FirebaseUtil
{
    public static async Task<string> GetUserIdFromToken(string idToken)
    {
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        return decoded.Uid;
    }
}