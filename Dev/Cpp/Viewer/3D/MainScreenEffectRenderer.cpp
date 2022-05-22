﻿
#include "MainScreenEffectRenderer.h"
#include "../3D/EffectRenderer.h"
#include "../EffekseerRendererCommon/EffekseerRenderer.PngTextureLoader.h"
#include "../EffekseerTool/EffekseerTool.Culling.h"
#include "../EffekseerTool/EffekseerTool.Grid.h"
#include "../EffekseerTool/EffekseerTool.Guide.h"
#include "../Graphics/GraphicsDevice.h"
#include <spdlog/sinks/basic_file_sink.h>
#include <spdlog/spdlog.h>

namespace Effekseer::Tool
{

MainScreenEffectRenderer::~MainScreenEffectRenderer()
{
	if (backgroundTexture_ != nullptr)
	{
		textureLoader_->Unload(backgroundTexture_);
		backgroundTexture_.Reset();
	}
}

void MainScreenEffectRenderer::OnAfterClear()
{
	EffectRenderer::OnAfterClear();
	
	const auto cameraMat = renderer_->GetCameraMatrix();
	const auto projMat = renderer_->GetProjectionMatrix();

	if (parameter_.RenderingMethod != Effekseer::Tool::RenderingMethodType::Overdraw)
	{
		if (IsGridShown)
		{
			grid_->SetLength(GridLength);
			grid_->IsShownXY = IsGridXYShown;
			grid_->IsShownXZ = IsGridXZShown;
			grid_->IsShownYZ = IsGridYZShown;
			grid_->Rendering(GridColor, IsRightHand, cameraMat, projMat);
		}

		{
			culling_->IsShown = IsCullingShown;
			culling_->Radius = CullingRadius;
			culling_->X = CullingPosition.X;
			culling_->Y = CullingPosition.Y;
			culling_->Z = CullingPosition.Z;
			culling_->Rendering(IsRightHand, cameraMat, projMat);
		}
	}
}

void MainScreenEffectRenderer::OnBeforePostprocess()
{
	if (RendersGuide)
	{
		guide_->Rendering(screenSize_.X, screenSize_.Y, GuideWidth, GuideHeight);
	}
}

bool MainScreenEffectRenderer::OnAfterInitialize()
{
	grid_ = std::shared_ptr<::EffekseerRenderer::Grid>(::EffekseerRenderer::Grid::Create(graphics_->GetGraphics()->GetGraphicsDevice()));
	if (grid_ == nullptr)
	{
		return false;
	}

	spdlog::trace("OK Grid");

	guide_ = std::shared_ptr<::EffekseerRenderer::Guide>(::EffekseerRenderer::Guide::Create(graphics_->GetGraphics()->GetGraphicsDevice()));
	if (guide_ == nullptr)
	{
		return false;
	}

	spdlog::trace("OK Guide");

	culling_ = std::shared_ptr<::EffekseerRenderer::Culling>(::EffekseerRenderer::Culling::Create(graphics_->GetGraphics()->GetGraphicsDevice()));
	if (culling_ == nullptr)
	{
		return false;
	}

	lineRenderer_ = std::make_shared<Effekseer::Tool::LineRenderer>(graphics_->GetGraphics()->GetGraphicsDevice());

	spdlog::trace("OK Culling");

	textureLoader_ = renderer_->CreateTextureLoader();

	return true;
}

void MainScreenEffectRenderer::StartRenderingLines()
{
	lineRenderer_->ClearCache();
}

void MainScreenEffectRenderer::AddLine(float p0x, float p0y, float p0z, float p1x, float p1y, float p1z, Effekseer::Tool::Color color)
{
	lineRenderer_->DrawLine(Vector3D{p0x, p0y, p0z}, Vector3D{p1x, p1y, p1z}, color);
}

void MainScreenEffectRenderer::EndRenderingLines(const Effekseer::Tool::Matrix44F& cameraMatrix, const Effekseer::Tool::Matrix44F& projectionMatrix)
{
	lineRenderer_->Render(cameraMatrix, projectionMatrix);
}

void MainScreenEffectRenderer::LoadBackgroundImage(const char16_t* path)
{
	if (backgroundPath == path)
		return;

	backgroundPath = path;

	if (backgroundTexture_ != nullptr)
	{
		textureLoader_->Unload(backgroundTexture_);
		backgroundTexture_.Reset();
	}

	backgroundTexture_ = textureLoader_->Load(path, Effekseer::TextureType::Color);
}

} // namespace Effekseer::Tool
