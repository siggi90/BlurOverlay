# BlurOverlay
The structure of your XAML must be something like this:

    <AbsoluteLayout x:Name="body_wrap" HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand">
        <StackLayout x:Name="content" AbsoluteLayout.LayoutBounds="0,0,1,1" AbsoluteLayout.LayoutFlags="All">
            <!-- place content here -->
        </StackLayout>
        <skia:SKCanvasView x:Name="blur_overlay" IsVisible="False" AbsoluteLayout.LayoutBounds="0,0,1,1" AbsoluteLayout.LayoutFlags="All">
        </skia:SKCanvasView>
        <StackLayout IsVisible="False" x:Name="black_overlay" AbsoluteLayout.LayoutBounds="0,0,1,1" AbsoluteLayout.LayoutFlags="All" StyleClass="black_overlay">
        </StackLayout>
    </AbsoluteLayout>

This way the blurred overlay appears above the content of your application, and the black overlay is semitransparent and appears above the blurred overlay. You must set the black overlay in the constructor of the blur. Example usage:

		this.blur = new blur(this, this.black_overlay);
		this.blur.init_blur();

		this.tapped(this.click, async (s, e) => {
			await this.blur.make_blur();
			await this.blur.display_blur();
		});

		this.tapped(this.black_overlay, async (s, e) => {
			await this.blur.hide_blur();
		});
