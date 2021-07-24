namespace WebApplication2
{
    // 미리 미리 인터페이스를 만들어서 필요한 레포지토리를 쉽게 바꿀 수 있어야 한다
    public interface ITestRepository
    {
        // 테스트용 프로젝트가 void로 되어 있지만 비동기(Task, async, await) 호출를 활용해야 한다
        void Test();
    }

    /// <summary>
    /// Repository는 내부 캐시, 데이터베이스, 레디스와 같이 저장소에 접근하는 프록시 역할을 한다
    /// </summary>
    public class TestRepository : ITestRepository
    {
        public void Test()
        {
            // db에 접근하여 값을 얻어 온다
        }
    }

    public class TestRepository_2 : ITestRepository
    {
        public void Test()
        {
            // 테스트용도로 사용. 내부 캐시에 접근하여 데이터를 얻어 온다
        }
    }
}
