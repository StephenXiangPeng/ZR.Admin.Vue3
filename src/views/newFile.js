import LineChart from './dashboard/LineChart';
import WordCloudChat from './dashboard/WordCloud.vue';
import dayjs from 'dayjs';
import duration from 'dayjs/plugin/duration';

export default (() => {
	const __VLS_setup = async () => {
		dayjs.extend(duration);

		import useUserStore from '@/store/modules/user';
		import useSocketStore from '@/store/modules/socket';
		import { getWeek } from '@/utils/ruoyi';
		const showEdit = ref(false);
		const data = {
			newVisitis: {
				expectedData: [100, 120, 161, 134, 105, 160, 165],
				actualData: [120, 82, 91, 154, 162, 140, 145]
			},
			messages: {
				expectedData: [200, 192, 120, 144, 160, 130, 140],
				actualData: [180, 160, 151, 106, 145, 150, 130]
			},
			purchases: {
				expectedData: [80, 100, 121, 104, 105, 90, 100],
				actualData: [120, 90, 100, 138, 142, 130, 130]
			},
			shoppings: {
				expectedData: [130, 140, 141, 142, 145, 150, 160],
				actualData: [120, 82, 91, 154, 162, 140, 130]
			}
		};
		const { proxy } = getCurrentInstance();
		const userInfo = computed(() => {
			return useUserStore().userInfo;
		});
		const onlineInfo = computed(() => {
			return useSocketStore().onlineInfo;
		});
		const currentTime = computed(() => {
			return proxy.parseTime(new Date(), 'YYYY-MM-DD');
		});
		const weekName = getWeek();

		let lineChartData = reactive([]);
		const dataType = ref(null);
		function handleSetLineChartData(type) {
			dataType.value = type;
			lineChartData = data[type];
		}
		handleSetLineChartData('newVisitis');

		function handleAdd() {
			proxy.$modal.msg('请通过搜索添加');
		}
		function workTimeFormatter(val) {
			return dayjs.duration(val * 60, 'second').format('HH时mm分');
		}
		const __VLS_publicComponent = (await import('vue')).defineComponent({
			setup() {
				return {};
			},
		});

		const __VLS_componentsOption = {};

		let __VLS_name!: 'copyindex_v2';
		function __VLS_template() {
			let __VLS_ctx!: InstanceType<import('./__VLS_types.js').PickNotAny<typeof __VLS_publicComponent, new () => {}>> & InstanceType<import('./__VLS_types.js').PickNotAny<typeof __VLS_internalComponent, new () => {}>> & {};
			/* Components */
			let __VLS_localComponents!: NonNullable<typeof __VLS_internalComponent extends { components: infer C; } ? C : {}> & typeof __VLS_componentsOption & typeof __VLS_ctx;
			let __VLS_otherComponents!: typeof __VLS_localComponents & import('./__VLS_types.js').GlobalComponents;
			let __VLS_own!: import('./__VLS_types.js').SelfComponent<typeof __VLS_name, typeof __VLS_internalComponent & typeof __VLS_publicComponent & (new () => { $slots: typeof __VLS_slots; }) >;
			let __VLS_components!: typeof __VLS_otherComponents & Omit<typeof __VLS_own, keyof typeof __VLS_otherComponents>;
			/* Style Scoped */
			type __VLS_StyleScopedClasses = {} &
			{ 'home'?: boolean; } &
			{ 'home-card-more'?: boolean; } &
			{ 'user-item'?: boolean; } &
			{ 'user-item-left'?: boolean; } &
			{ 'user-item-right'?: boolean; } &
			{ 'right-title'?: boolean; } &
			{ 'info'?: boolean; } &
			{ 'work-wrap'?: boolean; } &
			{ 'item'?: boolean; } &
			{ 'name'?: boolean; } &
			{ 'chart-wrapper'?: boolean; } &
			{ 'chart-wrapper'?: boolean; };
			let __VLS_styleScopedClasses!: __VLS_StyleScopedClasses | keyof __VLS_StyleScopedClasses | (keyof __VLS_StyleScopedClasses)[];
			/* CSS variable injection */
			/* CSS variable injection end */
			let __VLS_templateComponents!: {} &
				import('./__VLS_types.js').WithComponent<'ElRow', typeof __VLS_components, 'ElRow', 'elRow', 'el-row'> &
				import('./__VLS_types.js').WithComponent<'ElCol', typeof __VLS_components, 'ElCol', 'elCol', 'el-col'> &
				import('./__VLS_types.js').WithComponent<'LineChart', typeof __VLS_components, 'LineChart', 'lineChart', 'line-chart'>;
			__VLS_components.ElRow; __VLS_components.ElRow; __VLS_components.ElRow; __VLS_components.ElRow; __VLS_components.elRow; __VLS_components.elRow; __VLS_components.elRow; __VLS_components.elRow; __VLS_components['el-row']; __VLS_components['el-row']; __VLS_components['el-row']; __VLS_components['el-row'];
			// @ts-ignore
			[ElRow, ElRow, ElRow, ElRow,];
			__VLS_components.ElCol; __VLS_components.ElCol; __VLS_components.ElCol; __VLS_components.ElCol; __VLS_components.elCol; __VLS_components.elCol; __VLS_components.elCol; __VLS_components.elCol; __VLS_components['el-col']; __VLS_components['el-col']; __VLS_components['el-col']; __VLS_components['el-col'];
			// @ts-ignore
			[ElCol, ElCol, ElCol, ElCol,];
			__VLS_components.LineChart; __VLS_components.LineChart; __VLS_components.lineChart; __VLS_components.lineChart; __VLS_components['line-chart']; __VLS_components['line-chart'];
			// @ts-ignore
			[LineChart, LineChart,];
			{
				({} as JSX.IntrinsicElements).div;
				({} as JSX.IntrinsicElements).div;
				(__VLS_x as JSX.IntrinsicElements)['div'] = { class: ("home"), };
				{
					__VLS_templateComponents.ElRow;
					(__VLS_x as import('./__VLS_types.js').ComponentProps<typeof __VLS_templateComponents.ElRow>) = { gutter: ((32)), };
					{
						__VLS_templateComponents.ElCol;
						(__VLS_x as import('./__VLS_types.js').ComponentProps<typeof __VLS_templateComponents.ElCol>) = { xs: ((24)), sm: ((24)), lg: ((24)), };
						{
							({} as JSX.IntrinsicElements).div;
							({} as JSX.IntrinsicElements).div;
							(__VLS_x as JSX.IntrinsicElements)['div'] = { class: ("chart-wrapper"), };
							{
								(__VLS_x as import('./__VLS_types.js').ComponentProps<typeof __VLS_templateComponents.LineChart>) = { chartData: ((__VLS_ctx.lineChartData)), 'chart-data': ((__VLS_ctx.lineChartData)), key: ((__VLS_ctx.dataType)), };
								// @ts-ignore
								[lineChartData, dataType,];
							}
						}
					}
				}
				{
					__VLS_templateComponents.ElRow;
					(__VLS_x as import('./__VLS_types.js').ComponentProps<typeof __VLS_templateComponents.ElRow>) = { gutter: ((32)), };
					{
						__VLS_templateComponents.ElCol;
						(__VLS_x as import('./__VLS_types.js').ComponentProps<typeof __VLS_templateComponents.ElCol>) = { xs: ((24)), sm: ((24)), lg: ((24)), };
						{
							({} as JSX.IntrinsicElements).div;
							({} as JSX.IntrinsicElements).div;
							(__VLS_x as JSX.IntrinsicElements)['div'] = { class: (""), };
							{
								(__VLS_x as import('./__VLS_types.js').ComponentProps<typeof __VLS_templateComponents.LineChart>) = { chartData: ((__VLS_ctx.WordCloudChat)), 'chart-data': ((__VLS_ctx.WordCloudChat)), key: ((__VLS_ctx.dataType)), };
								// @ts-ignore
								[WordCloudChat, dataType,];
							}
						}
					}
				}
			}
			if (typeof __VLS_styleScopedClasses === 'object' && !Array.isArray(__VLS_styleScopedClasses)) {
				__VLS_styleScopedClasses['home'];
				__VLS_styleScopedClasses['chart-wrapper'];
			}
			declare var __VLS_slots: {};
			return __VLS_slots;
		}
		const __VLS_internalComponent = (await import('vue')).defineComponent({
			setup() {
				return {
					LineChart: LineChart,
					WordCloudChat: WordCloudChat,
					lineChartData: lineChartData,
					dataType: dataType,
				};
			},
		});
		return {} as typeof __VLS_publicComponent;
	};
	return {} as typeof __VLS_setup extends () => Promise<infer T> ? T : never;
})({} as any);
