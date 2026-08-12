## 짭무봇
디스코드 명란젓 서버 전용 봇

NetCord와 .NET NativeAOT를 사용합니다.

### 빌드
빌드에는 .NET 10 SDK가 필요합니다.

```powershell
dotnet build ChimusBot.sln -c Release
dotnet publish ChimusBot/ChimusBot.csproj -c Release -r win-x64 --self-contained true
```

배포 환경에 따라 `win-x64`를 `linux-x64`, `linux-arm64` 등의 RID로 변경하면 됩니다.

### 명령어 추가 방법
1. [이 프로젝트](https://github.com/daramkun/Chimus)를 Fork 한다.
2. Fork 한 저장소를 Clone 받아 수정하고 Commit하여 Push 한다.
3. 해당 변경사항을 이 프로젝트에 Pull requests로 적용 요청한다.
