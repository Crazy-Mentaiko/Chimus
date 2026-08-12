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

### LiteDB 데이터 변환

기존 `data.db`를 새 JSON 저장소로 한 번 변환합니다.

```powershell
dotnet run --project ChimusBot.DataMigration -- data.db data.json
```

출력 파일이 이미 존재하면 변환을 중단합니다. 의도적으로 덮어쓸 때만 마지막에 `--force`를 추가하세요.
봇의 기본 데이터 파일은 `data.json`이며, 기존 `db.path` 설정이나 `DB_PATH` 환경 변수로 다른 경로를 지정할 수 있습니다.

### 명령어 추가 방법
1. [이 프로젝트](https://github.com/daramkun/Chimus)를 Fork 한다.
2. Fork 한 저장소를 Clone 받아 수정하고 Commit하여 Push 한다.
3. 해당 변경사항을 이 프로젝트에 Pull requests로 적용 요청한다.
