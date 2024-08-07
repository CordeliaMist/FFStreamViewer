using Gagspeak.API.Data;
using Gagspeak.API.Dto.User;
using GagSpeak.API.Data.Character;
using GagSpeak.API.Dto.Connection;
using GagSpeak.API.Dto.Permissions;
using GagSpeak.API.Dto.UserPair;
using Microsoft.AspNetCore.SignalR.Client;

namespace FFStreamViewer.WebAPI;

#pragma warning disable MA0040
/// <summary> 
/// 
/// This partial class contains the user related functions.
/// 
/// User related functions are functions that our client calls to the server, sending it information via Dto's.
/// The server will then take those Dto's and handle the equivalent functions accordingly.
/// 
/// </summary>
public partial class ApiController
{
    /// <summary> 
    /// 
    /// Sends request to the server, asking to add the defined UserDto to the clients UserPair list.
    /// 
    /// </summary>
    /// <param name="user">the data transfer object of the User the client desires to add as a pair.</param>
    public async Task UserAddPair(UserDto user)
    {
        // if we are not connected, return
        if (!IsConnected) return;
        Logger.LogDebug("Adding pair {user} to client. Sending call to server.", user);
        // otherwise, call the UserAddPair function on the server with the user data transfer object via signalR
        await _gagspeakHub!.SendAsync(nameof(UserAddPair), user).ConfigureAwait(false); // wait for request to send.
    }

    /// <summary> 
    /// 
    /// Send a request to the server, asking it to remove the declared UserDto from the clients userPair list.
    /// 
    /// </summary>
    public async Task UserRemovePair(UserDto userDto)
    {
        // if we are not connected, return
        if (!IsConnected) return;
        // if we are connected, send the request to remove the user from the user pair list
        await _gagspeakHub!.SendAsync(nameof(UserRemovePair), userDto).ConfigureAwait(false);
    }

    /// <summary> 
    /// 
    /// Sends a request to the server, asking for the connected clients account to be deleted.
    /// 
    /// </summary>
    public async Task UserDelete()
    {
        // verify that we are connected
        CheckConnection();
        // send the account deletion request to the server
        await _gagspeakHub!.SendAsync(nameof(UserDelete)).ConfigureAwait(false);
        // perform a reconnect, because the account is no longer valid in the context of the current connection.
        await CreateConnections().ConfigureAwait(false);
    }

    /// <summary> 
    /// 
    /// Send a request to the server, asking it to return a list of all currently online users that you are paired with.
    /// 
    /// </summary>
    /// <returns>Returns a list of OnlineUserIdent Data Transfer Objects</returns>
    public async Task<List<OnlineUserIdentDto>> UserGetOnlinePairs()
    {
        return await _gagspeakHub!.InvokeAsync<List<OnlineUserIdentDto>>(nameof(UserGetOnlinePairs)).ConfigureAwait(false);
    }

    /// <summary> 
    /// 
    /// Send a request to the server, asking it to return a list of your paired clients.
    /// 
    /// </summary>
    /// <returns>Returns a list of UserPair data transfer objects</returns>
    public async Task<List<UserPairDto>> UserGetPairedClients()
    {
        return await _gagspeakHub!.InvokeAsync<List<UserPairDto>>(nameof(UserGetPairedClients)).ConfigureAwait(false);
    }

    /// <summary> 
    /// 
    /// Send a request to the server asking it to provide the profile of the user defined in the UserDto.
    /// 
    /// </summary>
    /// <returns>The user profile Dto belonging to the UserDto we sent</returns>
    public async Task<UserProfileDto> UserGetProfile(UserDto dto)
    {
        // if we are not connected, return a new user profile dto with the user data and disabled set to false
        if (!IsConnected) return new UserProfileDto(dto.User, Disabled: false, ProfilePictureBase64: null, Description: null);
        // otherwise, if we are connected, invoke the UserGetProfile function on the server with the user data transfer object
        return await _gagspeakHub!.InvokeAsync<UserProfileDto>(nameof(UserGetProfile), dto).ConfigureAwait(false);
    }

    /// <summary> 
    /// 
    /// Sets the profile of the client user, updating it to the clients paired users and the DB.
    /// 
    /// </summary>
    public async Task UserSetProfile(UserProfileDto userDescription)
    {
        if (!IsConnected) return;
        await _gagspeakHub!.InvokeAsync(nameof(UserSetProfile), userDescription).ConfigureAwait(false);
    }

    /// <summary>
    /// 
    /// Pushes the composite user data of a a character to other recipients.
    /// 
    /// (This is called upon by PushCharacterDataInternal, see bottom)
    /// 
    /// </summary>
    public async Task UserPushData(UserCharaCompositeDataMessageDto dto)
    {
        // try and push the character data dto to the server
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserPushData), dto).ConfigureAwait(false);
        }
        // if it failed, log it
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to Push character data");
        }
    }

    /// <summary>
    /// 
    /// Pushes the IPC data from the client character to other recipients.
    /// 
    /// </summary>
    public async Task UserPushDataIpc(UserCharaIpcDataMessageDto dto)
    {
        // try and push the character data dto to the server
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserPushDataIpc), dto).ConfigureAwait(false);
        }
        // if it failed, log it
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to Push character data");
        }
    }

    /// <summary>
    /// 
    /// Pushes the composite user data of a a character to other recipients.
    /// 
    /// </summary>
    public async Task UserPushDataAppearance(UserCharaAppearanceDataMessageDto dto)
    {
        // try and push the character data dto to the server
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserPushDataAppearance), dto).ConfigureAwait(false);
        }
        // if it failed, log it
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to Push character data");
        }
    }

    /// <summary>
    /// 
    /// Pushes the wardrobe data of the client to other recipients.
    /// 
    /// </summary>
    public async Task UserPushDataWardrobe(UserCharaWardrobeDataMessageDto dto)
    {
        // try and push the character data dto to the server
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserPushDataWardrobe), dto).ConfigureAwait(false);
        }
        // if it failed, log it
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to Push character data");
        }
    }

    /// <summary>
    /// 
    /// Pushes the puppeteer alias lists of the client to other recipients.
    /// 
    /// </summary>
    public async Task UserPushDataAlias(UserCharaAliasDataMessageDto dto)
    {
        // try and push the character data dto to the server
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserPushDataAlias), dto).ConfigureAwait(false);
        }
        // if it failed, log it
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to Push character data");
        }
    }

    /// <summary>
    /// 
    /// Pushes the toybox pattern & trigger information of the client to other recipients.
    /// 
    /// </summary>
    public async Task UserPushDataPattern(UserCharaPatternDataMessageDto dto)
    {
        // try and push the character data dto to the server
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserPushDataPattern), dto).ConfigureAwait(false);
        }
        // if it failed, log it
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to Push character data");
        }
    }

    /// <summary> Pushes to server a request to modify a global permissions of the client. </summary>
    public async Task UserUpdateOwnGlobalPerm(UserGlobalPermChangeDto userPermissions)
    {
        CheckConnection();
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserUpdateOwnGlobalPerm), userPermissions).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to set own global permission");
        }
    }


    /// <summary> Pushes to server a request to modify a global permissions of the client. </summary>
    public async Task UserUpdateOtherGlobalPerm(UserGlobalPermChangeDto userPermissions)
    {
        CheckConnection();
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserUpdateOtherGlobalPerm), userPermissions).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to set other userpair global permission");
        }
    }

    /// <summary> Pushes to server a request to modify a unique userpair related permission of the client. </summary>
    public async Task UserUpdateOwnPairPerm(UserPairPermChangeDto userPermissions)
    {
        CheckConnection();
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserUpdateOwnPairPerm), userPermissions).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to update clients permission for a userpair.");
        }
    }

    /// <summary> Pushes to server a request to modify a permission on one of the clients userPairs pair permissions  </summary>
    public async Task UserUpdateOtherPairPerm(UserPairPermChangeDto userPermissions)
    {
        CheckConnection();
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserUpdateOtherPairPerm), userPermissions).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to update a pair permission belonging to one of the clients userpairs.");
        }
    }

    /// <summary>
    /// pushes a request to update your edit access permissions for one of your userPairs.
    /// </summary>
    public async Task UserUpdateOwnPairPermAccess(UserPairAccessChangeDto userPermissions)
    {
        CheckConnection();
        try
        {
            await _gagspeakHub!.InvokeAsync(nameof(UserUpdateOwnPairPermAccess), userPermissions).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to update your edit access permission for one of your userPairs");
        }
    }


    /* --------------------- Character Data Handling --------------------- */

    /// <summary> 
    /// This will call upon characterDataInternal, which will handle the assignments and the recipients, storing them into the Dto
    /// </summary>
    /// <param name="data"> the data to be sent to the list of users </param>
    /// <param name="onlineCharacters"> the online characters the data will be sent to </param>
    public async Task PushCharacterCompositeData(CharacterCompositeData data, List<UserData> onlineCharacters)
    {
        if (!IsConnected) return;

        try // if connected, try to push the data to the server
        {
            Logger.LogDebug("Pushing Character Composite data to {visible}", string.Join(", ", onlineCharacters.Select(v => v.AliasOrUID)));
            await UserPushData(new(onlineCharacters, data)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { Logger.LogDebug("Upload operation was cancelled"); }
        catch (Exception ex) { Logger.LogWarning(ex, "Error during upload of composite data"); }
    }

    /// <summary>
    /// Pushes another characters (or self)'s updated IPC data to the list of visible recipients.
    /// </summary>
    /// <param name="data"> the data to be sent to the list of users </param>
    /// <param name="visibleCharacters"> the currently visible characters who will recieve the updated IPC data </param>
    public async Task PushCharacterIpcData(CharacterIPCData data, List<UserData> visibleCharacters)
    {
        if (!IsConnected) return;

        try // if connected, try to push the data to the server
        {
            Logger.LogDebug("Pushing Character IPC data to {visible}", string.Join(", ", visibleCharacters.Select(v => v.AliasOrUID)));
            await UserPushDataIpc(new(visibleCharacters, data)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { Logger.LogDebug("Upload operation was cancelled"); }
        catch (Exception ex) { Logger.LogWarning(ex, "Error during upload of IPC data"); }
    }


    /// <summary>
    /// Pushes another characters (or self)'s updated appearance data to the list of online recipients.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onlineCharacters"></param>
    public async Task PushCharacterAppearanceData(CharacterAppearanceData data, List<UserData> onlineCharacters)
    {
        if (!IsConnected) return;

        try // if connected, try to push the data to the server
        {
            Logger.LogDebug("Pushing Character Appearance data to {visible}", string.Join(", ", onlineCharacters.Select(v => v.AliasOrUID)));
            await UserPushDataAppearance(new(onlineCharacters, data)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { Logger.LogDebug("Upload operation was cancelled"); }
        catch (Exception ex) { Logger.LogWarning(ex, "Error during upload of Appearance data"); }
    }


    /// <summary>
    /// Pushes another characters (or self)'s updated wardrobe data to the list of online recipients.
    /// </summary>
    /// <param name="data"> the data to be sent to the list of users </param>
    /// <param name="onlineCharacters"> the online characters the data will be sent to </param>
    public async Task PushCharacterWardrobeData(CharacterWardrobeData data, List<UserData> onlineCharacters)
    {
        if (!IsConnected) return;

        try // if connected, try to push the data to the server
        {
            Logger.LogDebug("Pushing Character Wardrobe data to {visible}", string.Join(", ", onlineCharacters.Select(v => v.AliasOrUID)));
            await UserPushDataWardrobe(new(onlineCharacters, data)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { Logger.LogDebug("Upload operation was cancelled"); }
        catch (Exception ex) { Logger.LogWarning(ex, "Error during upload of Wardrobe data"); }
    }


    /// <summary>
    /// Pushes another characters (or self)'s updated alias list data to the list of online recipients.
    /// </summary>
    /// <param name="data"> the data to be sent to the list of users </param>
    /// <param name="onlineCharacters"> the online characters the data will be sent to </param>
    public async Task PushCharacterAliasListData(CharacterAliasData data, List<UserData> onlineCharacters)
    {
        if (!IsConnected) return;

        try // if connected, try to push the data to the server
        {
            Logger.LogDebug("Pushing Character Alias data to {visible}", string.Join(", ", onlineCharacters.Select(v => v.AliasOrUID)));
            await UserPushDataAlias(new(onlineCharacters, data)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { Logger.LogDebug("Upload operation was cancelled"); }
        catch (Exception ex) { Logger.LogWarning(ex, "Error during upload of Alias List data"); }
    }

    /// <summary>
    /// Pushes another characters (or self)'s updated pattern information to the list of online recipients.
    /// </summary>
    /// <param name="data"> the data to be sent to the list of users </param>
    /// <param name="onlineCharacters"> the online characters the data will be sent to </param>
    public async Task PushCharacterPatternInfoData(CharacterPatternInfo data, List<UserData> onlineCharacters)
    {
        if (!IsConnected) return;

        try // if connected, try to push the data to the server
        {
            Logger.LogDebug("Pushing Character PatternInfo to {visible}", string.Join(", ", onlineCharacters.Select(v => v.AliasOrUID)));
            await UserPushDataPattern(new(onlineCharacters, data)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { Logger.LogDebug("Upload operation was cancelled"); }
        catch (Exception ex) { Logger.LogWarning(ex, "Error during upload of Pattern Information"); }
    }
}
#pragma warning restore MA0040
