﻿using SteamNative;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	public struct Friend
	{
		public CSteamID Id;

		public Friend( CSteamID steamid )
		{
			Id = steamid;
		}

		public bool IsFriend => Relationship == FriendRelationship.Friend;
		public bool IsBlocked => Relationship == FriendRelationship.Blocked;
		public bool IsPlayingThisGame => GameInfo?.GameID == Utils.AppId;

		/// <summary>
		/// Returns true if this friend is online
		/// </summary>
		public bool IsOnline => State != PersonaState.Offline;

		/// <summary>
		/// Returns true if this friend is marked as away
		/// </summary>
		public bool IsAway => State == PersonaState.Away;

		/// <summary>
		/// Returns true if this friend is marked as busy
		/// </summary>
		public bool IsBusy => State == PersonaState.Busy;

		/// <summary>
		/// Returns true if this friend is marked as snoozing
		/// </summary>
		public bool IsSnoozing => State == PersonaState.Snooze;



		public FriendRelationship Relationship => Friends.Internal.GetFriendRelationship( Id );
		public PersonaState State => Friends.Internal.GetFriendPersonaState( Id );
		public string Name => Friends.Internal.GetFriendPersonaName( Id );
		public IEnumerable<string> NameHistory
		{
			get
			{
				for( int i=0; i<32; i++ )
				{
					var n = Friends.Internal.GetFriendPersonaNameHistory( Id, i );
					if ( string.IsNullOrEmpty( n ) )
						break;

					yield return n;
				}
			}
		}

		public int SteamLevel => Friends.Internal.GetFriendSteamLevel( Id );



		public FriendGameInfo? GameInfo
		{
			get
			{
				FriendGameInfo_t gameInfo = default( FriendGameInfo_t );
				if ( !Friends.Internal.GetFriendGamePlayed( Id, ref gameInfo ) )
					return null;

				return FriendGameInfo.From( gameInfo );
			}
		}

		public bool IsIn( CSteamID group_or_room )
		{
			return Friends.Internal.IsUserInSource( Id, group_or_room );
		}

		public struct FriendGameInfo
		{
			internal ulong GameID; // m_gameID class CGameID
			internal uint GameIP; // m_unGameIP uint32
			internal ushort GamePort; // m_usGamePort uint16
			internal ushort QueryPort; // m_usQueryPort uint16
			internal ulong SteamIDLobby; // m_steamIDLobby class CSteamID

			public static FriendGameInfo From( FriendGameInfo_t i )
			{
				return new FriendGameInfo
				{
					GameID = i.GameID,
					GameIP = i.GameIP,
					GamePort = i.GamePort,
					QueryPort = i.QueryPort,
					SteamIDLobby = i.SteamIDLobby,
				};
			}
		}

		public async Task<Image?> GetSmallAvatarAsync()
		{
			return await Friends.GetSmallAvatarAsync( Id );
		}

		public async Task<Image?> GetMediumAvatarAsync()
		{
			return await Friends.GetMediumAvatarAsync( Id );
		}

		public async Task<Image?> GetLargeAvatarAsync()
		{
			return await Friends.GetLargeAvatarAsync( Id );
		}

		public string GetRichPresence( string key )
		{
			var val = Friends.Internal.GetFriendRichPresence( Id, key );
			if ( string.IsNullOrEmpty( val ) ) return null;
			return val;
		}

		/// <summary>
		/// Invite this friend to the game that we are playing
		/// </summary>
		public bool InviteToGame( string Text )
		{
			return Friends.Internal.InviteUserToGame( Id, Text );
		}

		/// <summary>
		/// Sends a message to a Steam friend. Returns true if success
		/// </summary>
		public bool SendMessage( string message )
		{
			return Friends.Internal.ReplyToFriendMessage( Id, message );
		}

	}
}