#!/bin/bash

rootDir="."

(cd ${rootDir}/OAuthManager && dotnet run) &
cd ${rootDir}/SpotifyRandomPlaylists && dotnet run
