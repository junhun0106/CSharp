using WebApplication2;
using Microsoft.Extensions.Logging;

namespace WebService
{
    public interface ITestProvider
    {
    }

    public class TestProvider : ITestProvider
    {
        // 레포지토리끼리 서로가 서로를 호출하지 않도록
        // 기능 별로 Provider로 분리하여 호출하도록 하자
        private readonly ITestRepository _testRepository;

        // 로그는 외부 종속성을 최대한 피하고 LoggerFactory를 이용하자
        // appsettings에 Logging을 이용하여 필요한 로그만 골라내거나, 필요 없는 로그를 필터링하거나
        // 프로그램 실행 중에 config만 변경하는 것으로 로그를 필터링 할 수 있다
        private readonly ILogger _logger;

        public TestProvider(ILoggerFactory loggerFactory,
                            ITestRepository testRepository)
        {
            _logger = loggerFactory.CreateLogger<TestProvider>();
            _testRepository = testRepository;
        }

        public void Test()
        {
            _logger.LogDebug("test");
            _testRepository.Test();
        }
    }
}
