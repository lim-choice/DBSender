DBSender
2개의 DB에 접근해 SELECT 해온 데이터를 다른 곳으로 INSERT 하는 툴이다. XML의 정보를 읽어와 스크립트 형식으로 작성할 수 있게 간단히 짰다.

예) A 데이터베이스에 접근해 SELECT 정보를 가져온 후 가져온 정보를 B 데이터베이스에 INSERT 한다.

로그 관리용으로 쓰일 수 있지 않을까 하고 생각하며 제작 (윈10)

XML 구조는 아래와 같다.

Rule에 시간은 Spring의 crontab 파라미터가 인상깊어 비스무리하게 따라해봤다.

```
<?xml version="1.0" encoding="utf-8"?>
<Root>
  <!-- DB -->
  <Config>
    <!-- SQL 연결 설정 리스트 -->
    <SqlList>      
      <!-- SqlConfig를 name 구분으로 관리 -->
      <Sql name="Remote">
        <host value="127.0.0.1" />
        <port value="3306" />
        <db value="" />
        <uid value="" />
        <pwd value="" />
      </Sql>
      <Sql name="Local">
        <host value="127.0.0.1" />
        <port value="3306" />
        <db value="" />
        <uid value="" />
        <pwd value="" />
      </Sql>
    </SqlList>
    <!-- Query 설정 리스트 -->
    <QueryList>
      <!-- Query문을 name 구분으로 관리, 내부 innerText가 Query문 -->
      <Query name="S_UINFO">
        <![CDATA[
          select `id`, `name` from account
        ]]>
      </Query>
      <Query name="I_UINFO">
        <![CDATA[
          insert into account_log (`UID`, `NAME`)
        ]]>
      </Query>>
    </QueryList>
  </Config>

  <Rule>
    <!-- 타이머 설정 리스트 -->
    <TimerList>
      <!-- 타이머를 인터벌로 관리할 수 있음 -->
      <!--
        * cron param :
                       매 주기마다 실행 (기준: 초) 
                       해당 초에 실행 (기준: 초)
                       해당 분에 실행 (기준: 분)
                       해당 시에 실행 (기준: 시)
                       해당 일에 실행 (기준: 일)
                       해당 월에 실행 (기준: 월)
          
          (조건을 정하지 않으려면 *)
          
        * example) 
            60초마다 실행 : 60 * * * * *
            1시간마다 실행 : 3600 * * * * *
            매일 자정에 실행 : * 0 0 0 * *
            0시 0분에 5초마다 실행 : 5 * 0 0 * *
      -->
      <Timer name="테스트 로그 저장" cron="100 * * * * *" >
        <!-- Select 후 나온 값을 INSERT 한다. (가져온 값을 그대로 넣기 때문에 짝을 맞춰야 함.. DB to DB) -->
        <Select Sql="Remote" Query="S_UINFO" />
        <Insert Sql="Local" Query="I_UINFO" />
      </Timer>      
    </TimerList>
  </Rule>
  
</Root>

```
