using Microsoft.AspNetCore.Mvc;
using Moq;
using vuttr_api.domain.dtos;
using vuttr_api.presentation.controllers;
using vuttr_api.services.contracts;

namespace vuttr_api.tests.controllers;

public class ToolsControllerTests
{
    private ToolsController? _controller;
    private readonly Mock<IToolService>? _mockToolService;

    public ToolsControllerTests()
    {
        _mockToolService = new(MockBehavior.Loose);
    }
}